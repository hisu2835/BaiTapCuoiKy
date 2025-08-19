using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace BaiTapCuoiKy
{
    /// <summary>
    /// GameEffects - Visual and audio effects for DrawMaster Premium
    /// </summary>
    public static class GameEffects
    {
        #region Color Utilities

        /// <summary>
        /// Lighten a color by percentage
        /// </summary>
        public static Color LightenColor(Color color, float percentage)
        {
            return Color.FromArgb(
                color.A,
                Math.Min(255, (int)(color.R + (255 - color.R) * percentage)),
                Math.Min(255, (int)(color.G + (255 - color.G) * percentage)),
                Math.Min(255, (int)(color.B + (255 - color.B) * percentage))
            );
        }

        /// <summary>
        /// Darken a color by percentage
        /// </summary>
        public static Color DarkenColor(Color color, float percentage)
        {
            return Color.FromArgb(
                color.A,
                Math.Max(0, (int)(color.R * (1 - percentage))),
                Math.Max(0, (int)(color.G * (1 - percentage))),
                Math.Max(0, (int)(color.B * (1 - percentage)))
            );
        }

        #endregion

        #region Animation Helpers

        /// <summary>
        /// Tính toán bounce animation
        /// </summary>
        public static float CalculateBounce(float time, float amplitude = 1.0f)
        {
            return (float)(amplitude * Math.Abs(Math.Sin(time * Math.PI)));
        }

        /// <summary>
        /// Tính toán pulse animation
        /// </summary>
        public static float CalculatePulse(float time, float minScale = 0.8f, float maxScale = 1.2f)
        {
            var pulse = (float)((Math.Sin(time * Math.PI * 2) + 1) / 2);
            return minScale + (maxScale - minScale) * pulse;
        }

        #endregion

        #region Text Effects

        /// <summary>
        /// V? text v?i gradient effect
        /// </summary>
        public static void DrawGradientText(Graphics g, string text, Font font, Rectangle bounds, Color color1, Color color2)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            using (var brush = new LinearGradientBrush(
                new Point(bounds.X, bounds.Y), 
                new Point(bounds.X, bounds.Y + bounds.Height),
                color1, color2))
            {
                var format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                
                g.DrawString(text, font, brush, bounds, format);
            }
        }

        #endregion

        #region Visual Enhancements

        /// <summary>
        /// V? button v?i hi?u ?ng 3D
        /// </summary>
        public static void Draw3DButton(Graphics g, Rectangle bounds, Color baseColor, bool pressed = false)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            // Fill background
            using (var brush = new SolidBrush(baseColor))
            {
                g.FillRectangle(brush, bounds);
            }
            
            // Draw border with 3D effect
            var lightColor = LightenColor(baseColor, 0.3f);
            var darkColor = DarkenColor(baseColor, 0.3f);
            
            using (var lightPen = new Pen(lightColor, 1))
            using (var darkPen = new Pen(darkColor, 1))
            {
                if (!pressed)
                {
                    // Light border on top and left
                    g.DrawLine(lightPen, bounds.Left, bounds.Top, bounds.Right - 1, bounds.Top);
                    g.DrawLine(lightPen, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom - 1);
                    
                    // Dark border on bottom and right
                    g.DrawLine(darkPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom - 1);
                    g.DrawLine(darkPen, bounds.Left, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
                }
                else
                {
                    // Inverted for pressed effect
                    g.DrawLine(darkPen, bounds.Left, bounds.Top, bounds.Right - 1, bounds.Top);
                    g.DrawLine(darkPen, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom - 1);
                    
                    g.DrawLine(lightPen, bounds.Right - 1, bounds.Top, bounds.Right - 1, bounds.Bottom - 1);
                    g.DrawLine(lightPen, bounds.Left, bounds.Bottom - 1, bounds.Right - 1, bounds.Bottom - 1);
                }
            }
        }

        #endregion

        #region Sound Effect Helpers

        /// <summary>
        /// Play success sound effect
        /// </summary>
        public static void PlaySuccessSound()
        {
            try
            {
                // Simple system beep for success
                System.Media.SystemSounds.Asterisk.Play();
            }
            catch
            {
                // Ignore sound errors
            }
        }

        /// <summary>
        /// Play error sound effect
        /// </summary>
        public static void PlayErrorSound()
        {
            try
            {
                // Simple system beep for error
                System.Media.SystemSounds.Hand.Play();
            }
            catch
            {
                // Ignore sound errors
            }
        }

        /// <summary>
        /// Play warning sound
        /// </summary>
        public static void PlayWarningSound()
        {
            try
            {
                System.Media.SystemSounds.Exclamation.Play();
            }
            catch
            {
                // Ignore if sound fails
            }
        }

        #endregion
    }
}