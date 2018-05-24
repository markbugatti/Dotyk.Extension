// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Dotyk.Extension.Extensions.Logging.Outputs.Internal
{
    public struct LogMessageEntry
    {
        public string LevelString;
        public ConsoleColor? LevelBackground;
        public ConsoleColor? LevelForeground;
        public ConsoleColor? MessageColor;
        public string Message;
    }
}
