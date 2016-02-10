using System;

namespace GoCommando
{
    /// <summary>
    /// Apply this attribute to the class that has your <code>Main</code> method in order to have a nice banner printed out when the program starts
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BannerAttribute : Attribute
    {
        /// <summary>
        /// Gets the banner text
        /// </summary>
        public string BannerText { get; }

        /// <summary>
        /// Constructs the attribute
        /// </summary>
        public BannerAttribute(string bannerText)
        {
            BannerText = bannerText;
        }
    }
}