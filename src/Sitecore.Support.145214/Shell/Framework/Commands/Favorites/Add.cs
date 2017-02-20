using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Support.Shell.Framework.Commands.Favorites
{
    public class Add : Command
    {
        private static Item CreateFavorite(Item favorites, Item item)
        {
            Assert.ArgumentNotNull(favorites, "favorites");
            Assert.ArgumentNotNull(item, "item");
            ID itemId = new ID("{6073E5B2-8B53-4A0F-8353-C2DE393932AE}");
            TemplateItem item2 = Client.CoreDatabase.GetItem(itemId);
            Assert.IsNotNull(item2, "Template \"Favorite\" not found.");
            return favorites.Add(item.Name, item2);
        }

        public override void Execute(CommandContext context)
        {
            Item item = this.GetItem(context);
            if (item != null)
            {
                Context.Notifications.Disabled = true;
                try
                {
                    Item root = UserOptions.Favorites.Root;
                    if ((root != null) && (item != null))
                    {
                        string path = item.Paths.Path;
                        Item favorite = GetFavorite(root, path);
                        if (favorite == null)
                        {
                            favorite = CreateFavorite(root, item);
                        }
                        if (favorite != null)
                        {
                            favorite.Editing.BeginEdit();
                            favorite.Fields["Link"].Value = path;
                            favorite.Fields["Display name"].Value = item.DisplayName;
                            favorite.Appearance.Icon = item.Appearance.Icon;
                            favorite.Editing.EndEdit();
                            SheerResponse.Alert(Translate.Text("\"{0}\" has been added to your favorites.", new object[] { item.DisplayName }), new string[0]);
                        }
                    }
                }
                finally
                {
                    Context.Notifications.Disabled = false;
                }
            }
        }

        private static Item GetFavorite(Item favorites, string id)
        {
            Assert.ArgumentNotNull(favorites, "favorites");
            Assert.ArgumentNotNullOrEmpty(id, "id");
            foreach (Item item in favorites.Children)
            {
                string str = item["Link"];
                if (str == id)
                {
                    return item;
                }
            }
            return null;
        }

        private Item GetItem(CommandContext context)
        {
            Item item = null;
            string str = context.Parameters["id"];
            if (str != null)
            {
                string language = context.Parameters["language"];
                if (language == null)
                {
                    language = context.Items.Length == 1 ? context.Items[0].Language.Name : null;
                }
                Assert.IsNotNull(language, "language != null");
                string str3 = context.Parameters["version"];
                Sitecore.Data.Version version = (str3 != null) ? Sitecore.Data.Version.Parse(str3) : Sitecore.Data.Version.Latest;
                item = Context.ContentDatabase.GetItem(ID.Parse(str), Language.Parse(language), version);
            }
            if ((item == null) && (context.Items.Length == 1))
            {
                item = context.Items[0];
            }
            return item;
        }
    }
}