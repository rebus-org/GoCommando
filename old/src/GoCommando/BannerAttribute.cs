using System;

namespace GoCommando
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BannerAttribute : Attribute
    {
        public string Text { get; set; }

        public BannerAttribute(string text)
        {
            Text = text;
        }
    }
}