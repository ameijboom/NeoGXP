namespace NeoGXP.GXPEngine.LinAlg
{
    public interface IVec
    {
        public float GetElement();
        public float Magnitude();
        public float MagnitudeSquared();
        public IVec Scale(float scalar);
        public IVec Add(IVec vector);
        public float Dot(IVec vector);
        public float Cross(IVec vector);
        public IVec Multiply(IMatrix matrix);
    }
}