﻿using PokePedia.Models;

using System;
using System.Globalization;

using Xamarin.Forms;

namespace PokePedia.ViewModels.Converters
{
    class ElementTypeToColourConverter : IValueConverter
    {
        ElementalColours elementalColours = new ElementalColours();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return new SolidColorBrush(Color.FromHex(elementalColours.typeColour[(string)value]));
            }
            else
            {
                return new SolidColorBrush(Color.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
