using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Xml.Linq;

namespace ECS.Library.Render
{
    public class Atlas
    {
        public Texture2D Texture {get; set;}
        private Dictionary<string, TextureRegion> regions;

        public Atlas()
        {
            regions = new Dictionary<string, TextureRegion>();
        }

        public Atlas(Texture2D texture)
        {
            Texture = texture;
            regions = new Dictionary<string, TextureRegion>();
        }

        public void AddRegion(string name, int x, int y, int width, int height)
        {
            TextureRegion region = new TextureRegion(Texture, x, y, width, height);
            regions.Add(name, region);
        }

        public TextureRegion GetRegion(string name)
        {
            return regions[name];
        }

        public bool RemoveRegion(string name)
        {
            return regions.Remove(name);
        }

        public void Clear()
        {
            regions.Clear();
        }

        public static Atlas FromFile(ContentManager content, string fileName)
        {
            string filePath = Path.Combine(content.RootDirectory, fileName);
            using (Stream stream = TitleContainer.OpenStream(filePath))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    XDocument doc = XDocument.Load(reader);
                    XElement root = doc.Root;
                    string texturePath = root.Element("Texture").Value;
                    Atlas atlas = new Atlas(content.Load<Texture2D>(texturePath));

                    var regions = root.Element("Regions")?.Elements("Region");

                    if (regions != null)
                    {
                        foreach (var region in regions)
                        {
                            string name = region.Attribute("name")?.Value;
                            int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                            int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                            int width = int.Parse(region.Attribute("width")?.Value ?? "0");
                            int height = int.Parse(region.Attribute("height")?.Value ?? "0");

                            if (!string.IsNullOrEmpty(name))
                            {
                                atlas.AddRegion(name, x, y, width, height);
                            }
                        }
                    }
                    return atlas;
                }
            }
        }
    }
}