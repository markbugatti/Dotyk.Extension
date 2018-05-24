// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using Dotyk.Extension.Extensions.Logging.Outputs.Internal;
using Microsoft.VisualStudio.Shell;

namespace Dotyk.Extension.Logging.Outputs.Internal
{
    public class OutputsLoggerProcessor : IDisposable
    {
        private const int _maxQueuedMessages = 1024;

        private readonly BlockingCollection<LogMessageEntry> _messageQueue = new BlockingCollection<LogMessageEntry>(_maxQueuedMessages);
        private readonly Thread _outputThread;

        //public IOutputs Outputs;

        public OutputsLoggerProcessor()
        {
            // Start Outputs message queue processor
            _outputThread = new Thread(ProcessLogQueue)
            {
                IsBackground = true,
                Name = "Outputs logger queue processing thread"
            };
            _outputThread.Start();
        }

        public virtual void EnqueueMessage(LogMessageEntry message)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                try
                {
                    _messageQueue.Add(message);
                    return;
                }
                catch (InvalidOperationException) { }
            }

            // Adding is completed so just log the message
            WriteMessage(message);
        }

        // for testing
        internal virtual void WriteMessage(LogMessageEntry message)
        {
            //if (message.LevelString != null)
            //{
            //    Outputs.Write(message.LevelString, message.LevelBackground, message.LevelForeground);
            //}

            //Outputs.Write(message.Message, message.MessageColor, message.MessageColor);
            //Outputs.Flush();
            //TODO: change to outputWindows
            ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            OutputsWindow.outputPane.OutputString(message.Message + Environment.NewLine);
        }

        private void ProcessLogQueue()
        {
            try
            {
                foreach (var message in _messageQueue.GetConsumingEnumerable())
                {
                    WriteMessage(message);
                }
            }
            catch
            {
                try
                {
                    _messageQueue.CompleteAdding();
                }
                catch { }
            }
        }

        public void Dispose()
        {
            _messageQueue.CompleteAdding();

            try
            {
                _outputThread.Join(1500); // with timeout in-case Outputs is locked by user input
            }
            catch (ThreadStateException) { }
        }
    }
}
