﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TouchPointAnimated
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TouchPointAnimation2Page : ContentPage
    {
        bool pageIsActive;
        
        List<RipplingTouchPoint> _touchPoints;
        
        public TouchPointAnimation2Page()
        {
            InitializeComponent();

            _touchPoints = new List<RipplingTouchPoint>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            InitAnimation();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            pageIsActive = false;
        }

        private void SKCanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            var skImageInfo = e.Info;
            var skSurface = e.Surface;
            var skCanvas = skSurface.Canvas;

            var skCanvasWidth = skImageInfo.Width;
            var skCanvasHeight = skImageInfo.Height;

            skCanvas.Clear();

            //skCanvas.Translate((float)skCanvasWidth / 2, (float)skCanvasHeight / 2);

            foreach (var item in _touchPoints)
            {
                item.AnimatingRadius++;

                item.StrokeAlpha--;

                using (SKPaint paintTouchPoint = new SKPaint())
                {
                    paintTouchPoint.Style = SKPaintStyle.Stroke;
                    paintTouchPoint.StrokeWidth = 10;
                    paintTouchPoint.Color = SKColors.Red.WithAlpha((byte)item.StrokeAlpha);
                    skCanvas.DrawCircle(
                        item.TouchPointLocation.X,
                        item.TouchPointLocation.Y,
                        item.AnimatingRadius,
                        paintTouchPoint);
                }
            }

            // remove ripple once it's disappeared
            _touchPoints.RemoveAll(x => x.StrokeAlpha == 0);
        }

        private SKPoint _lastTouchPoint = new SKPoint();
        private void CanvasView_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            if (e.ActionType == SkiaSharp.Views.Forms.SKTouchAction.Pressed)
            {
                _lastTouchPoint = e.Location;
                e.Handled = true;
            }

            _lastTouchPoint = e.Location;

            _touchPoints.Add(
                new RipplingTouchPoint
                {
                    TouchPointLocation = _lastTouchPoint,
                }
            );

            CanvasView.InvalidateSurface();
        }

        private void InitAnimation()
        {
            pageIsActive = true;

            Device.StartTimer(TimeSpan.FromSeconds(1.0 / 30), () => {
                
                CanvasView.InvalidateSurface();

                return pageIsActive;
            });
        }
    }


    public class RipplingTouchPoint
    {
        public float AnimatingRadius { get; set; } = 1;

        public float StrokeAlpha { get; set; } = 255;

        public SKPoint TouchPointLocation { get; set; }
    }
}