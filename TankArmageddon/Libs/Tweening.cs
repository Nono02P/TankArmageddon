using Microsoft.Xna.Framework;
using System;

namespace TankArmageddon
{   
    public class Tweening
    {
        private delegate float TweenFunction(Tweening tweening);

        #region Enumérations
        public enum Tween
        {
            Linear,
            InQuad, OutQuad, InOutQuad, OutInQuad,
            InCubic, OutCubic, InOutCubic, OutInCubic,
            InQuart, OutQuart, InOutQuart, OutInQuart,
            InQuint, OutQuint, InOutQuint, OutInQuint,
            InSine, OutSine, InOutSine, OutInSine,
            InExpo, OutExpo, InOutExpo, OutInExpo,
            InCirc, OutCirc, InOutCirc, OutInCirc,
            InElastic, OutElastic, InOutElastic, OutInElastic,
            InBack, OutBack, InOutBack, OutInBack,
            InBounce, OutBounce, InOutBounce, OutInBounce,
        }
        #endregion

        #region Variables privées
        private TweenFunction _tweenFunction;
        #endregion

        #region Propriétés
        public Tween TweenType { get; private set; }
        public TimeSpan ElapsedTime { get; private set; } = TimeSpan.Zero;
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public TimeSpan Period { get; set; } = TimeSpan.Zero;
        public float Begin { get; set; }
        public float Ending { get; set; }
        public float Amplitud { get; set; } 
        public float Overshoot { get; set; } = 1.70158f;
        public bool Finished { get; set; }
        public float Result { get; private set; }
        #endregion

        #region Constructeur
        public Tweening(Tween pTweenType, int pBegin, int pDestination, TimeSpan pDuration)
        {
            TweenType = pTweenType;
            Begin = pBegin;
            Ending = pDestination - Begin;
            Duration = pDuration;

            AffectFunctions();
        }

        public Tweening(Tween pTweenType, int pBegin, int pDestination, TimeSpan pDuration, float pAmplitud, TimeSpan pPeriod)
        {
            TweenType = pTweenType;
            Begin = pBegin;
            Ending = pDestination - Begin;
            Duration = pDuration;
            Period = pPeriod;
            Amplitud = pAmplitud;

            AffectFunctions();
        }

        public Tweening(Tween pTweenType, int pBegin, int pDestination, TimeSpan pDuration, float pOvershoot)
        {
            TweenType = pTweenType;
            Begin = pBegin;
            Ending = pDestination - Begin;
            Duration = pDuration;
            Overshoot = pOvershoot;

            AffectFunctions();
        }
        #endregion

        #region Méthodes
        public void Update(GameTime gameTime)
        {
            Result = _tweenFunction(this);
            if (!Finished)
            {
                ElapsedTime += gameTime.ElapsedGameTime;
                if (ElapsedTime >= Duration)
                {
                    Finished = true;
                }
            }
        }

        private void AffectFunctions()
        {
            switch (TweenType)
            {
                case Tween.Linear:
                    _tweenFunction = TweenFunctions.Linear;
                    break;
                case Tween.InQuad:
                    _tweenFunction = TweenFunctions.InQuad;
                    break;
                case Tween.OutQuad:
                    _tweenFunction = TweenFunctions.OutQuad;
                    break;
                case Tween.InOutQuad:
                    _tweenFunction = TweenFunctions.InOutQuad;
                    break;
                case Tween.OutInQuad:
                    _tweenFunction = TweenFunctions.OutInQuad;
                    break;
                case Tween.InCubic:
                    _tweenFunction = TweenFunctions.InCubic;
                    break;
                case Tween.OutCubic:
                    _tweenFunction = TweenFunctions.OutCubic;
                    break;
                case Tween.InOutCubic:
                    _tweenFunction = TweenFunctions.InOutCubic;
                    break;
                case Tween.OutInCubic:
                    _tweenFunction = TweenFunctions.OutInCubic;
                    break;
                case Tween.InQuart:
                    _tweenFunction = TweenFunctions.InQuart;
                    break;
                case Tween.OutQuart:
                    _tweenFunction = TweenFunctions.OutQuart;
                    break;
                case Tween.InOutQuart:
                    _tweenFunction = TweenFunctions.InOutQuart;
                    break;
                case Tween.OutInQuart:
                    _tweenFunction = TweenFunctions.OutInQuart;
                    break;
                case Tween.InQuint:
                    _tweenFunction = TweenFunctions.InQuint;
                    break;
                case Tween.OutQuint:
                    _tweenFunction = TweenFunctions.OutQuint;
                    break;
                case Tween.InOutQuint:
                    _tweenFunction = TweenFunctions.InOutQuint;
                    break;
                case Tween.OutInQuint:
                    _tweenFunction = TweenFunctions.OutInQuint;
                    break;
                case Tween.InSine:
                    _tweenFunction = TweenFunctions.InSine;
                    break;
                case Tween.OutSine:
                    _tweenFunction = TweenFunctions.OutSine;
                    break;
                case Tween.InOutSine:
                    _tweenFunction = TweenFunctions.InOutSine;
                    break;
                case Tween.OutInSine:
                    _tweenFunction = TweenFunctions.OutInSine;
                    break;
                case Tween.InExpo:
                    _tweenFunction = TweenFunctions.InExpo;
                    break;
                case Tween.OutExpo:
                    _tweenFunction = TweenFunctions.OutExpo;
                    break;
                case Tween.InOutExpo:
                    _tweenFunction = TweenFunctions.InOutExpo;
                    break;
                case Tween.OutInExpo:
                    _tweenFunction = TweenFunctions.OutInExpo;
                    break;
                case Tween.InCirc:
                    _tweenFunction = TweenFunctions.InCirc;
                    break;
                case Tween.OutCirc:
                    _tweenFunction = TweenFunctions.OutCirc;
                    break;
                case Tween.InOutCirc:
                    _tweenFunction = TweenFunctions.InOutCirc;
                    break;
                case Tween.OutInCirc:
                    _tweenFunction = TweenFunctions.OutInCirc;
                    break;
                case Tween.InElastic:
                    _tweenFunction = TweenFunctions.InElastic;
                    break;
                case Tween.OutElastic:
                    _tweenFunction = TweenFunctions.OutElastic;
                    break;
                case Tween.InOutElastic:
                    _tweenFunction = TweenFunctions.InOutElastic;
                    break;
                case Tween.OutInElastic:
                    _tweenFunction = TweenFunctions.OutInElastic;
                    break;
                case Tween.InBack:
                    _tweenFunction = TweenFunctions.InBack;
                    break;
                case Tween.OutBack:
                    _tweenFunction = TweenFunctions.OutBack;
                    break;
                case Tween.InOutBack:
                    _tweenFunction = TweenFunctions.InOutBack;
                    break;
                case Tween.OutInBack:
                    _tweenFunction = TweenFunctions.OutInBack;
                    break;
                case Tween.InBounce:
                    _tweenFunction = TweenFunctions.InBounce;
                    break;
                case Tween.OutBounce:
                    _tweenFunction = TweenFunctions.OutBounce;
                    break;
                case Tween.InOutBounce:
                    _tweenFunction = TweenFunctions.InOutBounce;
                    break;
                case Tween.OutInBounce:
                    _tweenFunction = TweenFunctions.OutInBounce;
                    break;
                default:
                    throw new ArgumentNullException("Type de Tween non géré dans le constructeur");
            }
        }
        #endregion

        private static class TweenFunctions
        {
            #region Affectation des fonctions
            public static readonly TweenFunction Linear = functionLinear;

            public static readonly TweenFunction InQuad = functionInQuad;
            public static readonly TweenFunction OutQuad = functionOutQuad;
            public static readonly TweenFunction InOutQuad = functionInOutQuad;
            public static readonly TweenFunction OutInQuad = functionOutInQuad;

            public static readonly TweenFunction InCubic = functionInCubic;
            public static readonly TweenFunction OutCubic = functionOutCubic;
            public static readonly TweenFunction InOutCubic = functionInOutCubic;
            public static readonly TweenFunction OutInCubic = functionOutInCubic;

            public static readonly TweenFunction InQuart = functionInQuart;
            public static readonly TweenFunction OutQuart = functionOutQuart;
            public static readonly TweenFunction InOutQuart = functionInOutQuart;
            public static readonly TweenFunction OutInQuart = functionOutInQuart;

            public static readonly TweenFunction InQuint = functionInQuint;
            public static readonly TweenFunction OutQuint = functionOutQuint;
            public static readonly TweenFunction InOutQuint = functionInOutQuint;
            public static readonly TweenFunction OutInQuint = functionOutInQuint;

            public static readonly TweenFunction InSine = functionInSine;
            public static readonly TweenFunction OutSine = functionOutSine;
            public static readonly TweenFunction InOutSine = functionInOutSine;
            public static readonly TweenFunction OutInSine = functionOutInSine;

            public static readonly TweenFunction InExpo = functionInExpo;
            public static readonly TweenFunction OutExpo = functionOutExpo;
            public static readonly TweenFunction InOutExpo = functionInOutExpo;
            public static readonly TweenFunction OutInExpo = functionOutInExpo;

            public static readonly TweenFunction InCirc = functionInCirc;
            public static readonly TweenFunction OutCirc = functionOutCirc;
            public static readonly TweenFunction InOutCirc = functionInOutCirc;
            public static readonly TweenFunction OutInCirc = functionOutInCirc;

            public static readonly TweenFunction InElastic = functionInElastic;
            public static readonly TweenFunction OutElastic = functionOutElastic;
            public static readonly TweenFunction InOutElastic = functionInOutElastic;
            public static readonly TweenFunction OutInElastic = functionOutInElastic;

            public static readonly TweenFunction InBack = functionInBack;
            public static readonly TweenFunction OutBack = functionOutBack;
            public static readonly TweenFunction InOutBack = functionInOutBack;
            public static readonly TweenFunction OutInBack = functionOutInBack;

            public static readonly TweenFunction InBounce = functionInBounce;
            public static readonly TweenFunction OutBounce = functionOutBounce;
            public static readonly TweenFunction InOutBounce = functionInOutBounce;
            public static readonly TweenFunction OutInBounce = functionOutInBounce;
            #endregion

            #region Linear
            private static float functionLinear(float t, float b, float c, float d) { return c * t / d + b; }
            private static float functionLinear(Tweening tn)
            {
                return functionLinear((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            #endregion

            #region Quad
            private static float functionInQuad(float t, float b, float c, float d)
            {
                t /= d;
                return (float)(c * Math.Pow(t, 2) + b);
            }
            private static float functionInQuad(Tweening tn)
            {
                return functionInQuad((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionOutQuad(float t, float b, float c, float d)
            {
                t /= d;
                return -c * t * (t - 2) + b;
            }
            private static float functionOutQuad(Tweening tn)
            {
                return functionOutQuad((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionInOutQuad(float t, float b, float c, float d)
            {
                t /= d * 2;
                if (t < 1)
                {
                    return (float)(c / 2 * Math.Pow(t, 2) + b);
                }
                else
                {
                    return -c / 2 * ((t - 1) * (t - 3) - 1) + b;
                }
            }
            private static float functionInOutQuad(Tweening tn)
            {
                return functionInOutQuad((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionOutInQuad(float t, float b, float c, float d)
            {
                if (t < d / 2)
                {
                    return functionOutQuad(t * 2, b, c / 2, d);
                }    
                else
                {
                    return functionInQuad((t * 2) - d, b + c / 2, c / 2, d);
                }
            }
            private static float functionOutInQuad(Tweening tn)
            {
                return functionOutInQuad((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            #endregion

            #region Cubic
            private static float functionInCubic(float t, float b, float c, float d)
            {
                t /= d;
                return (float)(c * Math.Pow(t, 3) + b);
            }
            private static float functionInCubic(Tweening tn)
            {
                return functionInCubic((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionOutCubic(float t, float b, float c, float d)
            {
                t = t / d - 1;
                return (float)(c * (Math.Pow(t, 3) + 1) + b);
            }
            private static float functionOutCubic(Tweening tn)
            {
                return functionOutCubic((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionInOutCubic(float t, float b, float c, float d)
            {
                t /= d * 2;
                if (t < 1)
                {
                    return (c / 2 * t * t * t + b);
                }
                else
                {
                    t -= 2;
                    return c / 2 * (t * t * t + 2) + b;
                }
            }
            private static float functionInOutCubic(Tweening tn)
            {
                return functionInOutCubic((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionOutInCubic(float t, float b, float c, float d)
            {
                if (t < d / 2)
                {
                    return functionOutCubic(t * 2, b, c / 2, d);
                }
                else
                {
                    return functionInCubic((t * 2) - d, b + c / 2, c / 2, d);
                }
            }
            private static float functionOutInCubic(Tweening tn)
            {
                return functionOutInCubic((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            #endregion

            #region Quart
            private static float functionInQuart(float t, float b, float c, float d)
            {
                t /= d;
                return (float)(c * Math.Pow(t, 4) + b);
            }
            private static float functionInQuart(Tweening tn)
            {
                return functionInQuart((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionOutQuart(float t, float b, float c, float d)
            {
                t = t / d - 1;
                return (float)(-c * (Math.Pow(t, 4) - 1) + b);
            }
            private static float functionOutQuart(Tweening tn)
            {
                return functionOutQuart((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionInOutQuart(float t, float b, float c, float d)
            {
                t /= d * 2;
                if (t < 1)
                {
                    return (float)(c / 2 * Math.Pow(t, 4) + b);
                }
                else
                {
                    t -= 2;
                    return (float)(-c / 2 * (Math.Pow(t, 4) - 2) + b);
                }
            }
            private static float functionInOutQuart(Tweening tn)
            {
                return functionInOutQuart((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionOutInQuart(float t, float b, float c, float d)
            {
                if (t < d / 2)
                {
                    return functionOutQuart(t * 2, b, c / 2, d);
                }
                else
                {
                    return functionInQuart((t * 2) - d, b + c / 2, c / 2, d);
                }
            }
            private static float functionOutInQuart(Tweening tn)
            {
                return functionOutInQuart((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            #endregion

            #region Quint
            private static float functionInQuint(float t, float b, float c, float d)
            {
                t /= d;
                return (float)(c * Math.Pow(t, 5) + b);
            }
            private static float functionInQuint(Tweening tn)
            {
                return functionInQuint((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionOutQuint(float t, float b, float c, float d)
            {
                t = t / d - 1;
                return (float)(c * (Math.Pow(t, 5) + 1) + b);
            }
            private static float functionOutQuint(Tweening tn)
            {
                return functionOutQuint((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionInOutQuint(float t, float b, float c, float d)
            {
                t /= d * 2;
                if (t < 1)
                {
                    return (float)(c / 2 * Math.Pow(t, 5) + b);
                }
                else
                {
                    t -= 2;
                    return (float)(c / 2 * (Math.Pow(t, 5) + 2) + b);
                }
            }
            private static float functionInOutQuint(Tweening tn)
            {
                return functionInOutQuint((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionOutInQuint(float t, float b, float c, float d)
            {
                if (t < d / 2)
                {
                    return functionOutQuint(t * 2, b, c / 2, d);
                }
                else
                {
                    return functionInQuint((t * 2) - d, b + c / 2, c / 2, d);
                }
            }
            private static float functionOutInQuint(Tweening tn)
            {
                return functionOutInQuint((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            #endregion

            #region Sine
            private static float functionInSine(float t, float b, float c, float d)
            {
                return (float)(-c * Math.Cos(t / d * (Math.PI / 2)) + c + b);
            }
            private static float functionInSine(Tweening tn)
            {
                return functionInSine((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionOutSine(float t, float b, float c, float d)
            {
                return (float)(c * Math.Sin(t / d * (Math.PI / 2)) + b);
            }
            private static float functionOutSine(Tweening tn)
            {
                return functionOutSine((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionInOutSine(float t, float b, float c, float d)
            {
                return (float)(-c / 2 * (Math.Cos(Math.PI * t / d) - 1) + b);
            }
            private static float functionInOutSine(Tweening tn)
            {
                return functionInOutSine((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionOutInSine(float t, float b, float c, float d)
            {
                if (t < d / 2)
                {
                    return functionOutSine(t * 2, b, c / 2, d);
                }
                else
                {
                    return functionInSine((t * 2) - d, b + c / 2, c / 2, d);
                }
            }
            private static float functionOutInSine(Tweening tn)
            {
                return functionOutInSine((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            #endregion

            #region Expo
            private static float functionInExpo(float t, float b, float c, float d)
            {
                if (t == 0)
                {
                    return b;
                }
                else
                {
                    return (float)(c * Math.Pow(2, 10 * (t / d - 1)) + b - c * 0.001);
                }
            }
            private static float functionInExpo(Tweening tn)
            {
                return functionInExpo((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionOutExpo(float t, float b, float c, float d)
            {
                if (t == d)
                {
                    return b + c;
                }
                else
                {
                    return (float)(c * 1.001 * (-Math.Pow(2, -10 * t / d) + 1) + b);
                }
            }
            private static float functionOutExpo(Tweening tn)
            {
                return functionOutExpo((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionInOutExpo(float t, float b, float c, float d)
            {
                if (t == 0)
                    return b;
                if (t == d)
                    return b + c;
                t /= d * 2;
                if (t < 1)
                {
                    return (float)(c / 2 * Math.Pow(2, 10 * (t - 1)) + b - c * 0.0005);
                }
                else
                {
                    t--;
                    return (float)(c / 2 * 1.0005 * (-Math.Pow(2, -10 * t) + 2) + b);
                }
            }
            private static float functionInOutExpo(Tweening tn)
            {
                return functionInOutExpo((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionOutInExpo(float t, float b, float c, float d)
            {
                if (t < d / 2)
                {
                    return functionOutExpo(t * 2, b, c / 2, d);
                }
                else
                {
                    return functionInExpo((t * 2) - d, b + c / 2, c / 2, d);
                }
            }
            private static float functionOutInExpo(Tweening tn)
            {
                return functionOutInExpo((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            #endregion

            #region Circ
            private static float functionInCirc(float t, float b, float c, float d)
            {
                t /= d;
                return (float)(-c * (Math.Sqrt(1 - Math.Pow(t, 2)) - 1) + b);
            }
            private static float functionInCirc(Tweening tn)
            {
                return functionInCirc((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionOutCirc(float t, float b, float c, float d)
            {
                t = t / d - 1;
                return (float)(c * Math.Sqrt(1 - Math.Pow(t, 2)) + b);
            }
            private static float functionOutCirc(Tweening tn)
            {
                return functionOutCirc((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionInOutCirc(float t, float b, float c, float d)
            {
                t /= d * 2;
                if (t < 1)
                {
                    return (float)(-c / 2 * (Math.Sqrt(1 - t * t) - 1) + b);
                }
                else
                {
                    t -= 2;
                    return (float)(c / 2 * (Math.Sqrt(1 - t * t) + 1) + b);
                }
            }
            private static float functionInOutCirc(Tweening tn)
            {
                return functionInOutCirc((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionOutInCirc(float t, float b, float c, float d)
            {
                if (t < d / 2)
                {
                    return functionOutCirc(t * 2, b, c / 2, d);
                }
                else
                {
                    return functionInCirc((t * 2) - d, b + c / 2, c / 2, d);
                }
            }
            private static float functionOutInCirc(Tweening tn)
            {
                return functionOutInCirc((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            #endregion

            #region Elastic
            private static float functionInElastic(float t, float b, float c, float d, float a, float p)
            {
                if (t == 0)
                    return b;
                t /= d;
                if (t == 1)
                    return b + c;
                if (p == 0)
                    p = d * 0.3f;

                float s;
                if (a == 0 || a < Math.Abs(c))
                {
                    a = c;
                    s = p / 4;
                }
                else
                {
                    s = (float)(p / (2 * Math.PI) * Math.Asin(c / a));
                }
                t--;
                return (float)(-(a * Math.Pow(2, 10 * t) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b);
            }
            private static float functionInElastic(Tweening tn)
            {
                return functionInElastic((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds, tn.Amplitud, (float)tn.Period.TotalMilliseconds);
            }
            private static float functionOutElastic(float t, float b, float c, float d, float a, float p)
            {
                if (t == 0)
                    return b;
                t /= d;
                if (t == 1)
                    return b + c;
                if (p == 0)
                    p = d * 0.3f;
                float s;

                if (a == 0 || a < Math.Abs(c))
                {
                    a = c;
                    s = p / 4;
                }
                else
                {
                    s = (float)(p / (2 * Math.PI) * Math.Asin(c / a));
                }
                return (float)(a * Math.Pow(2, -10 * t) * Math.Sin((t * d - s) * (2 * Math.PI) / p) + c + b);
            }
            private static float functionOutElastic(Tweening tn)
            {
                return functionOutElastic((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds, tn.Amplitud, (float)tn.Period.TotalMilliseconds);
            }
            private static float functionInOutElastic(float t, float b, float c, float d, float a, float p)
            {
                if (t == 0)
                    return b;
                t /= d * 2;
                if (t == 2)
                    return b + c;
                if (p == 0)
                    p = d * (0.3f * 1.5f);
                float s;
                if (a == 0 || a < Math.Abs(c))
                {
                    a = c;
                    s = p / 4;
                }
                else
                {
                    s = (float)(p / (2 * Math.PI) * Math.Asin(c / a));
                }
                if (t < 1)
                {
                    t--;
                    return (float)(-0.5 * (a * Math.Pow(2, 10 * t) * Math.Sin((t * d - s) * (2 * Math.PI) / p)) + b);
                }
                else
                {
                    t--;
                    return (float)(a * Math.Pow(2, -10 * t) * Math.Sin((t * d - s) * (2 * Math.PI) / p) * 0.5 + c + b);
                }
            }
            private static float functionInOutElastic(Tweening tn)
            {
                return functionInOutElastic((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds, tn.Amplitud, (float)tn.Period.TotalMilliseconds);
            }
            private static float functionOutInElastic(float t, float b, float c, float d, float a, float p)
            {
                if (t < d / 2)
                {
                    return functionOutElastic(t * 2, b, c / 2, d, a, p);
                }
                else
                {
                    return functionInElastic((t * 2) - d, b + c / 2, c / 2, d, a, p);
                }
            }
            private static float functionOutInElastic(Tweening tn)
            {
                return functionOutInElastic((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds, tn.Amplitud, (float)tn.Period.TotalMilliseconds);
            }
            #endregion

            #region Back
            private static float functionInBack(float t, float b, float c, float d, float s)
            {
                if (s == 0)
                    s = 1.70158f;
                t /= d;
                return c * t * t * ((s + 1) * t - s) + b;
            }
            private static float functionInBack(Tweening tn)
            {
                return functionInBack((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds, tn.Overshoot);
            }
            private static float functionOutBack(float t, float b, float c, float d, float s)
            {
                if (s == 0)
                    s = 1.70158f;
                t = t / d - 1;
                return c * (t * t * ((s + 1) * t + s) + 1) + b;
            }
            private static float functionOutBack(Tweening tn)
            {
                return functionOutBack((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds, tn.Overshoot);
            }
            private static float functionInOutBack(float t, float b, float c, float d, float s)
            {
                if (s == 0)
                    s = 1.70158f;
                s = s * 1.525f;
                t /= d * 2;
                if (t < 1)
                {
                    return c / 2 * (t * t * ((s + 1) * t - s)) + b;
                }
                else
                {
                    t -= 2;
                    return c / 2 * (t * t * ((s + 1) * t + s) + 2) + b;
                }
            }
            private static float functionInOutBack(Tweening tn)
            {
                return functionInOutBack((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds, tn.Overshoot);
            }
            private static float functionOutInBack(float t, float b, float c, float d, float s)
            {
                if (t < d / 2)
                {
                    return functionOutBack(t * 2, b, c / 2, d, s);
                }
                else
                {
                    return functionInBack((t * 2) - d, b + c / 2, c / 2, d, s);
                }
            }
            private static float functionOutInBack(Tweening tn)
            {
                return functionOutInBack((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds, tn.Overshoot);
            }
            #endregion

            #region Bounce
            private static float functionInBounce(float t, float b, float c, float d)
            {
                return c - functionOutBounce(d - t, 0, c, d) + b;
            }
            private static float functionInBounce(Tweening tn)
            {
                return functionInBounce((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionOutBounce(float t, float b, float c, float d)
            {
                t /= d;
                if (t < 1 / 2.75f)
                {
                    return c * (7.5625f * t * t) + b;
                }
                else if (t < 2 / 2.75f)
                {
                    t -= (1.5f / 2.75f);
                    return c * (7.5625f * t * t + 0.75f) + b;
                }
                else if (t < 2.5f / 2.75f)
                {
                    t -= (2.25f / 2.75f);
                    return c * (7.5625f * t * t + 0.9375f) + b;
                }
                else
                {
                    t -= (2.625f / 2.75f);
                    return c * (7.5625f * t * t + 0.984375f) + b;
                }
            }
            private static float functionOutBounce(Tweening tn)
            {
                return functionOutBounce((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionInOutBounce(float t, float b, float c, float d)
            {
                if (t < d / 2)
                {
                    return functionInBounce(t * 2, 0, c, d) * 0.5f + b;
                }
                else
                {
                    return functionOutBounce(t * 2 - d, 0, c, d) * 0.5f + c * 0.5f + b;
                }
            }
            private static float functionInOutBounce(Tweening tn)
            {
                return functionInOutBounce((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            private static float functionOutInBounce(float t, float b, float c, float d)
            {
                if (t < d / 2)
                {
                    return functionOutBounce(t * 2, b, c / 2, d);
                }
                else
                {
                    return functionInBounce((t * 2) - d, b + c / 2, c / 2, d);
                }
            }
            private static float functionOutInBounce(Tweening tn)
            {
                return functionOutInBounce((float)tn.ElapsedTime.TotalMilliseconds, tn.Begin, tn.Ending, (float)tn.Duration.TotalMilliseconds);
            }
            #endregion
        }
    }
}