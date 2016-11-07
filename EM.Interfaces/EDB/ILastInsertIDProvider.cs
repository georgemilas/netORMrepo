using System;
using System.Collections.Generic;
using System.Text;

namespace EM.DB
{
    public interface ILastInsertIDProvider
    {
        string lastInsertID { get; set; }
    }
}
