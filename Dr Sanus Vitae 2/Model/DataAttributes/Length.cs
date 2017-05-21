using System;

namespace SanusVitae
{
    public class LengthAttribute : Attribute
    {
        private readonly int length_;
        public LengthAttribute(int length)
        {
            length_ = length;
        }
        public int Length { get { return length_; } }
    }
}
