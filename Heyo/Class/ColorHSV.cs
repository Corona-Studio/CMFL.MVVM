namespace Heyo.Class
{
    /// <summary>
    ///     类      名：ColorHSV
    ///     功      能：H 色相 \ S 饱和度(纯度) \ V 明度 颜色模型
    ///     日      期：2015-01-22
    ///     修      改：2015-03-20
    ///     作      者：ls9512
    /// </summary>
    public class ColorHSV
    {
        private int _a;
        private int _h;
        private int _s;
        private int _v;

        /// <summary>
        ///     构造方法
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        public ColorHSV(int h, int s, int v)
        {
            _a = 255;
            _h = h;
            _s = s;
            _v = v;
        }

        public ColorHSV(int a, int h, int s, int v)
        {
            _a = a;
            _h = h;
            _s = s;
            _v = v;
        }

        /// <summary>
        ///     色相
        /// </summary>
        public int H
        {
            get => _h;
            set
            {
                _h = value;
                _h = _h > 360 ? 360 : _h;
                _h = _h < 0 ? 0 : _h;
            }
        }

        /// <summary>
        ///     饱和度(纯度)
        /// </summary>
        public int S
        {
            get => _s;
            set
            {
                _s = value;
                _s = _s > 255 ? 255 : _s;
                _s = _s < 0 ? 0 : _s;
            }
        }

        /// <summary>
        ///     明度
        /// </summary>
        public int V
        {
            get => _v;
            set
            {
                _v = value;
                _v = _v > 255 ? 255 : _v;
                _v = _v < 0 ? 0 : _v;
            }
        }


        /// <summary>
        ///     Alpha
        /// </summary>
        public int A
        {
            get => _a;
            set
            {
                _a = value;
                _a = _a > 255 ? 255 : _a;
                _a = _a < 0 ? 0 : _a;
            }
        }
    }
}