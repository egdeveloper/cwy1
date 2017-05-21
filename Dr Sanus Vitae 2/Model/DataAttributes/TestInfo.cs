using System;
using System.ComponentModel;

namespace SanusVitae
{
    public class TestInfo : DisplayNameAttribute
    {
        private readonly Unity unity_;
        private readonly bool visible_;
        public TestInfo(string display_name, Unity unity, bool visible)
            : base(display_name)
        {
            unity_ = unity;
            visible_ = visible;
        }
        public Unity Unity { get { return unity_; } }
        public bool Visible { get { return visible_; } }
    }
}
