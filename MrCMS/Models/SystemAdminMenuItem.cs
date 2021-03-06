using MrCMS.ACL.Rules;
using MrCMS.Website;

namespace MrCMS.Models
{
    public class SystemAdminMenuItem : IAdminMenuItem
    {
        private SubMenu _children;
        public string Text => "System";
        public string IconClass => "fa fa-cogs";
        public string Url { get; private set; }

        public bool CanShow =>
            new SystemAdminMenuACL().CanAccess(CurrentRequestData.CurrentUser, SystemAdminMenuACL.ShowMenu);

        public SubMenu Children => _children ??
                                   (_children = GetChildren());

        public int DisplayOrder => 100;

        private static SubMenu GetChildren()
        {
            var systemAdminMenuACL = new SystemAdminMenuACL();
            return new SubMenu
            {
                new ChildMenuItem("Settings", "#", subMenu: new SubMenu
                {
                    new ChildMenuItem("Site Settings", "/Admin/Settings",
                        ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.SiteSettings)),
                    new ChildMenuItem("System Settings", "/Admin/SystemSettings",
                        ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.SiteSettings)),
                    new ChildMenuItem("Filesystem Settings", "/Admin/Settings/FileSystem",
                        ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.FileSystemSettings)),
                    new ChildMenuItem("Mail Settings", "/Admin/SystemSettings/Mail",
                        ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.SiteSettings)),
                    new ChildMenuItem("ACL", "/Admin/ACL",
                        ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.ACL))
                }),
                new ChildMenuItem("Security", "#", ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Security),
                    new SubMenu
                    {
                        new ChildMenuItem("Custom Scripts", "/Admin/CustomScriptPages",
                            ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Security)),
                        new ChildMenuItem("Security Options", "/Admin/SecurityOptions",
                            ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Security))
                    }),
                new ChildMenuItem("Import/Export Documents", "/Admin/ImportExport/Documents",
                    ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.ImportExport)),
                new ChildMenuItem("Message Templates", "/Admin/MessageTemplate",
                    ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.MessageTemplates)),
                new ChildMenuItem("Page Templates", "/Admin/PageTemplate",
                    ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.PageTemplates)),
                new ChildMenuItem("Page Defaults", "/Admin/PageDefaults",
                    ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.UrlGenerators)),
                new ChildMenuItem("Sites", "/Admin/Sites",
                    ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Sites)),
                new ChildMenuItem("Resources", "/Admin/Resource",
                    ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Resources)),
                new ChildMenuItem("Logs", "/Admin/Log",
                    ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Logs)),
                new ChildMenuItem("Batches", "/Admin/Batch",
                    ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Batch)),
                new ChildMenuItem("Tasks", "/Admin/Task",
                    ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Tasks)),
                new ChildMenuItem("Indexes", "/Admin/Indexes",
                    ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Indices)),
                new ChildMenuItem("Message Queue", "/Admin/MessageQueue",
                    ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.MessageQueue)),
                new ChildMenuItem("Notifications", "/Admin/Notification",
                    ACLOption.Create(systemAdminMenuACL, SystemAdminMenuACL.Notifications)),
                new ChildMenuItem("Clear Caches", "/Admin/ClearCaches"),
                new ChildMenuItem("About", "/Admin/About")
            };
        }
    }
}