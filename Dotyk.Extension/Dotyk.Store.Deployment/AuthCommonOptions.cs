using System;
using System.Linq;
using System.Threading.Tasks;
//using Dotyk.Store.Model;
using Microsoft.Extensions.Logging;

namespace Dotyk.Store.Cli
{
    public abstract class AuthCommonOptions : ServerCommonOptions
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool Register { get; set; }
        public bool UseDotykMe { get; set; }
        public string DotykMeToken { get; set; }
    }
}
