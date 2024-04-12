using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;
using System.Windows.Media;
using System.Windows;

namespace MouseRecording.Shaders
{
	public class HeatColorizer : ShaderEffect
	{
		public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(HeatColorizer), 0);
		public static readonly DependencyProperty PaletteProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Palette", typeof(HeatColorizer), 1);

		public HeatColorizer()
		{
			PixelShader = new PixelShader();
			PixelShader.UriSource = new Uri("HeatColorizer.ps", UriKind.Relative);
			UpdateShaderValue(InputProperty);
			UpdateShaderValue(PaletteProperty);
		}

		public Brush Input
		{
			get { return (Brush)GetValue(InputProperty); }
			set { SetValue(InputProperty, value); }
		}

		public Brush Palette
		{
			get { return (Brush)GetValue(PaletteProperty); }
			set { SetValue(PaletteProperty, value); }
		}
	}
}
