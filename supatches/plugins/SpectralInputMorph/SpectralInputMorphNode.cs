#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "InputMorph", Category = "Transform Spectral", Help = "Linear interpolation between a spread of transform", Tags = "matrix")]
	#endregion PluginInfo
    public class SpectralInputMorphNode : IPluginEvaluate
	{
		#region fields & pins
        [Input("Switch", IsSingle = true, MinValue = 0, MaxValue = 1)]
        public ISpread<float> FSwitch;

		[Input("Input")]
		public ISpread<Matrix4x4> FInput;
		
        //TODO: Generalize plugin to support bin size
        //[Input("Bin Size", DefaultValue = -1, IsSingle = true)]
        //public IDiffSpread<int> FBinSize;

		[Output("Output")]
		public ISpread<Matrix4x4> FOutput;

		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
            ////Compute bins (negative number means that we specify the number of spreads countainted in the input)
            //int nbSpread = FBinSize[0] < 0 ? -FBinSize[0] : FInput.SliceCount / FBinSize[0];
            //int binSize = FBinSize[0] < 0 ? -FBinSize[0] : FBinSize[0];

            //Find the 2 values closest to the switch location
            float pos = FSwitch[0] * (FInput.SliceCount-1);
            int pos0 = (int)pos;
            int pos1 = pos0 + 1;
            float offset = pos - pos0;

            //Lerp every matrix member
            double[] v = new double[16];
            for (int j = 0; j < 16; ++j)
            {
                v[j] = VMath.Lerp(FInput[pos0].Values[j], FInput[pos1].Values[j], offset);
            }


            //Single output
            FOutput.SliceCount = 1;

            FOutput[0] = new Matrix4x4(v[0],v[1],v[2],v[3],
                                       v[4],v[5],v[6],v[7],
                                       v[8],v[9],v[10],v[11],
                                       v[12],v[13],v[14],v[15]);

			//FLogger.Log(LogType.Debug, "Logging to Renderer (TTY)");
		}
	}
}
