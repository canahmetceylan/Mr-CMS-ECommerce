using System.ComponentModel;

namespace MrCMS.Web.Apps.Amazon.Models
{
    public enum AmazonLogStatus
    {
        [Description("Initiating operation")]
        Initiation,
        [Description("Stage")]
        Stage,
        [Description("Operation successfully completed")]
        Completion,
        [Description("Error happened")]
        Error,
        [Description("New item inserted")]
        Insert,
        [Description("Item(s) successfully updated")]
        Update,
        [Description("Item(s) deleted")]
        Delete
    }
}