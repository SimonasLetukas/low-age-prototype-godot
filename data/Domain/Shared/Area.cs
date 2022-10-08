namespace low_age_data.Domain.Shared
{
    public struct Rectangle
    {
        public static readonly Rectangle Empty = new Rectangle();

        private int x; // Do not rename (binary serialization) 
        private int y; // Do not rename (binary serialization) 
        private int width; // Do not rename (binary serialization) 
        private int height; // Do not rename (binary serialization) 
        
        public Rectangle(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }
}