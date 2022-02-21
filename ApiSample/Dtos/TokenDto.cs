using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSample.Dtos
{
    

    /// <summary>
    /// End User döneceğimiz response
    /// </summary>
    public class TokenDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpireDateSeconds { get; set; } = 3600; 
        public string TokenType { get; set; } = "Bearer";

    }
}
