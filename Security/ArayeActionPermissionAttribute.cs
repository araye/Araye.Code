using System;

namespace Araye.Code.Security
{
    public class ArayeActionPermissionAttribute : Attribute
    {
        public string Title { get; set; }
        public string ActionId { get; set; }
        public bool HaveSamePermission { get; set; }
    }
}