using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace CrazyMiniWar_server
{
    class Map
    {
        public List<Vector2> Positions = new List<Vector2>();

        public static Map Load(String file)
        {
            Map loaded;
            StreamReader reader = new StreamReader(file);
            String text = reader.ReadToEnd();
            loaded = JsonConvert.DeserializeObject<Map>(text);
            return loaded;
        }

        public String Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
