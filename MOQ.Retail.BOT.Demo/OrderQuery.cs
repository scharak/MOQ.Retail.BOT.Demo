using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MOQ.Retail.BOT.Demo
{
 
        [Serializable]
        public class OrderQuery
        {
            [Prompt("Please enter your {&}")]
            public string OrderID { get; set; }


        }
    
}