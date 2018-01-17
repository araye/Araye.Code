using System;

namespace Araye.Code.Models
{
    public class BaseEntity
    {
        public DateTime? DateCreated { get; set; }
        public string UserCreated { get; set; }
        public string UserCreatedIP { get; set; }
        public DateTime? DateModified { get; set; }
        public string UserModified { get; set; }
        public string UserModifiedIP { get; set; }
    }
}
