﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Fractal_Designer
{
    partial class MainWindow
    {
        FractalColourer Colourer;
        ComplexFunction Function = z => z * z * z - 2;
        ComplexFunction AlgorithmFunction;

        private Complex GetComplexCoords(Point point)
        {
            int re = (int) point.X;
            int im = (int) point.Y;

            int lengthReal = (int) Fractal.ActualWidth;
            int lengthImaginary = (int) Fractal.ActualHeight;

            double radiusReal = (double) Settings.Instance.radius;
            double radiusImaginary = ((double) Settings.Instance.radius) * lengthImaginary / lengthReal;
            if (lengthReal < lengthImaginary)
            {
                radiusReal = ((double) Settings.Instance.radius) * lengthReal / lengthImaginary;
                radiusImaginary = (double) Settings.Instance.radius;
            }
            double realPosition = Settings.Instance.center.Real + (re * 2d - lengthReal) / lengthReal * radiusReal;
            double imaginaryPosition = Settings.Instance.center.Imaginary + (im * 2d - lengthImaginary) / lengthImaginary * radiusImaginary;
            return new Complex(realPosition, imaginaryPosition);
        }

        private void ComputeFractal(Complex complexCoordinates)
        {
            switch (Settings.Instance.drageffect)
            {
                case 0:
                    Settings.Instance.center += MouseLastClickComplex - complexCoordinates;
                    break;
                case 1:
                    AlgorithmFunction = z => Function(z) * (z - complexCoordinates);
                    break;
                case 2:
                    AlgorithmFunction = z => Function(z) * (z - complexCoordinates) * (z - complexCoordinates) * (z - complexCoordinates);
                    break;
                case 3:
                    AlgorithmFunction = z => Function(z) * (z.Magnitude - complexCoordinates.Magnitude);
                    break;
                case 4:
                    AlgorithmFunction = z => Function(z) / (z - complexCoordinates);
                    break;
                default:
                    break;
            }
            ComputeFractal();
        }

        private void ComputeFractal()/* => ComputeFractal(GetComplexCoords(Mouse.GetPosition(Fractal)));*/
        {
            if (Settings.Instance.center == null || double.IsNaN(Fractal.ActualWidth) || double.IsNaN(Fractal.ActualHeight) || Fractal.Width == 0 || Fractal.Height == 0)
                return;

            var fractalFactory = new ComplexFractalFactory();
            Colourer = new FractalColourer(fractalFactory.GetAutoConfiguredAlgorithmByID(Settings.Instance.algorithm, AlgorithmFunction ?? Function));

            AsyncDraw(Colourer, Settings.Instance.center, (double) Settings.Instance.radius, (int) Fractal.Width, (int) Fractal.Height);

            Status.Text = $"Radius={(double) Settings.Instance.radius}";
        }

    }
}
