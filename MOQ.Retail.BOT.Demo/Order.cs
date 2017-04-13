using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MOQ.Retail.BOT.Demo
{
    using System;

    [Serializable]
    public class Order
    {
        public string ID{ get; set; }

        public string ProductName { get; set; }

        public string CustomerID { get; set; }

        public int Quantity { get; set; }

        public float UnitPrice { get; set; }

        public float TotalPrice { get; set; }

        public float AmountPaid { get; set; }

        public float Balance { get; set; }

    }
}