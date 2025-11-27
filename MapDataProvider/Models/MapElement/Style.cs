using Newtonsoft.Json;
using System;
using System.Drawing;

namespace MapDataProvider.Models.MapElement
{
    public class Style
    {
        public SolidBrush Fill { get; set; }

        [JsonConverter(typeof(PanConverter))]
        public Pen Stroke { get; set; }
    }

    public class MyPen
    {
        [JsonConstructor]
        public MyPen(Color color, float width, float[] dash)
        {
            Color = color;
            Width = width;
            DashPattern = dash;
        }
        public MyPen(Pen pen)
        {
            Color = pen.Color;
            Width = pen.Width;
            try
            {
                DashPattern = pen.DashPattern ?? new float[] { 1 };
            }
            catch
            {
                // Якщо не вдається отримати DashPattern, використовуємо суцільну лінію
                DashPattern = new float[] { 1 };
            }
        }
        [JsonProperty("color")]
        public Color Color { get; set; }

        [JsonProperty("width")]
        public float Width { get; set; }

        [JsonProperty("dash")]
        public float[] DashPattern { get; set; }

        public Pen ToPan() => new Pen(Color, Width) { DashPattern = DashPattern };

    }

    public class PanConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var pen = (Pen)value;
            var myPen = new MyPen(pen);
            string penString = JsonConvert.SerializeObject(myPen);
            writer.WriteValue(penString);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            MyPen myPen = JsonConvert.DeserializeObject<MyPen>((string)reader.Value);
            return myPen.ToPan();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MyPen);
        }
    }
}
