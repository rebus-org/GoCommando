using System;

namespace GoCommando
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BannerAttribute : Attribute
    {
        public string BannerText { get; }

        public BannerAttribute(string bannerText)
        {
            BannerText = bannerText;
        }
    }
}