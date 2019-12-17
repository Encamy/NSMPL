using System;
using System.Collections.Generic;
using System.Text;

namespace SMPL
{
    class NormalRandom : Random
    {
        double m_previousValue = double.NaN;
        protected override double Sample()
        {
            if (!double.IsNaN(m_previousValue))
            {
                double result = m_previousValue;
                m_previousValue = double.NaN;
                return result;
            }

            double u, v, s;
            do
            {
                u = 2 * base.Sample() - 1;
                v = 2 * base.Sample() - 1;
                s = u * u + v * v;
            }
            while (u <= -1 || v <= -1 || s >= 1 || s == 0);
            double r = Math.Sqrt(-2 * Math.Log(s) / s);

            m_previousValue = r * v;
            return r * u;
        }
    }
}
