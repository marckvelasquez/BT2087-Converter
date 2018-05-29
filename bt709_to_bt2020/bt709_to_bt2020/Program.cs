using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecimalMath;

namespace bt709_to_bt2020
{
    class Program
    {
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // Digital YCBCR values to Normalized nonlinear ycbcr
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        static double[] inv_quantization_ycbcr(ref double[] input_ycbcr)
        {
            double[] nonlinear_ycbcr = new double[3];
            nonlinear_ycbcr[0] = (input_ycbcr[0] / 4 - 16) / 219;
            nonlinear_ycbcr[1] = (input_ycbcr[1] / 4 - 16) / 219;
            nonlinear_ycbcr[2] = (input_ycbcr[2] / 4 - 16) / 219;
            return nonlinear_ycbcr;
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // NORMAL YCBCR TO NONLINEAR RGB
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        static decimal[] norm_ycbcr_to_rgb709(ref double[] nonlinear_ycbcr)
        {
            decimal[] nonlinear_rgb709 = new decimal[3] { 0, 0, 0 };
            decimal[][] transfer_matrix = new decimal[3][]
            {
                new decimal[] { (decimal)1 , (decimal)0 , (decimal)1.5748} ,
                new decimal[] { (decimal)1 , (decimal)-0.18732 , (decimal)-0.46812} ,
                new decimal[] { (decimal)1 , (decimal)-1.8556 , (decimal)0 }
            };

            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 1; j++ )
                {
                    for( int k = 0; k < 3; k++)
                    {
                        nonlinear_rgb709[i] = nonlinear_rgb709[i] + transfer_matrix[i][k] * (decimal) nonlinear_ycbcr[k];
                    }
                } //end of j
            }// end of i


            return nonlinear_rgb709;
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // Inverse Gamma 709 RGB -> RGB
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        static decimal[] inverse_gamma_709(ref decimal[] nonlinear_rgb709)
        {
            decimal[] linear_rgb709 = new decimal[3] { 0, 0, 0 };
            for( int i = 0; i < 3; i++)
            {
                linear_rgb709[i] = DecimalEx.Pow( Math.Abs(nonlinear_rgb709[i]) , (decimal)2.4) ;
            }

            return linear_rgb709;
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // Linear 709 RGB to Linear 2020
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        static decimal[] linear_709_to_2020(ref decimal[] linear_rgb709)
        {
            decimal[] linear_rgb2020 = new decimal[3] { 0, 0, 0 };
            decimal[][] transfer_matrix = new decimal[3][] { 
                new decimal[]{0.6274m	,	0.3293m	,	0.0433m},
                new decimal[]{0.0691m	,	0.9195m ,	0.0114m	},
                new decimal[]{0.0164m	,	0.0880m	,	0.8956m}};
            // Matrix multiplication for [3,1] and [3,3]
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 1; j++)
                {
                    for (int k = 0; k < 3; k++)
                        linear_rgb2020[i] = linear_rgb2020[i] + (transfer_matrix[i][k] * linear_rgb709[k]);
                }
            }

            return linear_rgb2020;
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // Linear 2020 To NonLinear 2020
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        static decimal[] Inverse_gamma_2020(ref decimal[] linear_rgb2020)
        {
            decimal[] nonlinear_rgb2020 = new decimal[3] { 0, 0, 0 };
            for (int i = 0; i < 3; i++)
            {
                nonlinear_rgb2020[i] = DecimalEx.Pow(Math.Abs(linear_rgb2020[i]), (decimal)(1 / 2.4));
            }

            return nonlinear_rgb2020;
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // NonLinear 2020 to YCBCR 2020
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        static decimal[] Rgb2020_to_ycbcr2020(ref decimal[] nonlinear_rgb2020)
        {
            decimal[] ycbcr2020 = new decimal[3] { 0, 0, 0 };
            decimal[][] transfer_matrix = new decimal[3][] {
                new decimal[] {0.2627m	,	0.6780m	,	0.0593m},
                new decimal[] {-0.13963m	,	-0.36037m ,	0.5000m	}, 
                new decimal[] {0.500m	,	-0.45979m	,	-0.04021m}
            };


            // Matrix multiplication for [3,1] and [3,3]
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 1; j++)
                {
                    for (int k = 0; k < 3; k++)
                        ycbcr2020[i] = ycbcr2020[i] + ( transfer_matrix[i][k] * nonlinear_rgb2020[k]);
		            }
	            }
            return ycbcr2020;
        }

        static decimal[] YCBCR_to_10bit(ref decimal[] nonlinear_ycbcr2020)
        {
            decimal[] decimal_ycbcr_10bit = new decimal[3];
            decimal_ycbcr_10bit[0] = Decimal.Round((nonlinear_ycbcr2020[0] * 219 + 16) * 4);
            decimal_ycbcr_10bit[1] = Decimal.Round((nonlinear_ycbcr2020[1] * 224 + 128) * 4);
            decimal_ycbcr_10bit[2] = Decimal.Round((nonlinear_ycbcr2020[2] * 224 + 128) * 4);
            return decimal_ycbcr_10bit;
        }
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        // Main Function
        // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

        static void Main(string[] args)
        {
            double[] input_codes = new double[3] { 250, 409, 960 };
            double[] nonlinear_ycbcr = new double[3];
            decimal[] nonlinear_rgb709 = new decimal[3];
            decimal[] linear_rgb709 = new decimal[3];
            decimal[] linear_rgb2020 = new decimal[3];
            decimal[] nonlinear_rgb2020 = new decimal[3];
            decimal[] nonlinear_ycbcr2020 = new decimal[3];
            decimal[] decimal_ycbcr_10bit = new decimal[3];

            nonlinear_ycbcr = inv_quantization_ycbcr(ref input_codes);
            nonlinear_rgb709 = norm_ycbcr_to_rgb709(ref nonlinear_ycbcr);
            linear_rgb709 = inverse_gamma_709(ref nonlinear_rgb709);
            linear_rgb2020 = linear_709_to_2020(ref linear_rgb709);
            nonlinear_rgb2020 = Inverse_gamma_2020(ref linear_rgb2020);
            nonlinear_ycbcr2020 = Rgb2020_to_ycbcr2020(ref nonlinear_rgb2020);
            decimal_ycbcr_10bit = YCBCR_to_10bit(ref nonlinear_ycbcr2020);

            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(input_codes[i]);
                Console.WriteLine(nonlinear_ycbcr[i]);
                Console.WriteLine(nonlinear_rgb709[i]);
                Console.WriteLine(linear_rgb709[i]);
                Console.WriteLine(linear_rgb2020[i]);
                Console.WriteLine(nonlinear_rgb2020[i]);
                Console.WriteLine(nonlinear_ycbcr2020[i]);
                Console.WriteLine(decimal_ycbcr_10bit[i]);
                Console.WriteLine("\n");
            }
        }
    }
}
