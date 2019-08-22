using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VectorGraphics;

public class Sheep : Animal
{
    protected override void Start ()
    {
        base.Start();
        health = 20;
    }

    protected override void SetAppearance ()
    {
        string sheepSvg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 25.55 15.48"">
	            <g><path id=""Body"" d=""M19.59,9.45A4.27,4.27,0,0,0,19,7.12a2.3,2.3,0,0,0-2.07-1.07A2.47,2.47,0,0,0,15.34,7a7.41,7.41,0,0,0-.94,1.58,6.48,6.48,0,0,1-1.21-1,3.41,3.41,0,0,0-2.62-.83A2.59,2.59,0,0,0,8.48,8.36a.36.36,0,0,1-.11.2c-.08,0-.19,0-.28,0a7.84,7.84,0,0,0-2-.51,1.06,1.06,0,0,0-.47,0,1.15,1.15,0,0,0-.41.35,4.28,4.28,0,0,0-1,2,2,2,0,0,0,.84,2,2.15,2.15,0,0,0,1.34,4,2.36,2.36,0,0,0,.06,1.83,2.12,2.12,0,0,0,1.28.77,9.55,9.55,0,0,0,1.84.3,2.41,2.41,0,0,0,1.3-.21,1,1,0,0,0,.53-1.11,8.92,8.92,0,0,0,3.4,1.59,3.83,3.83,0,0,0,2.07,0,1.84,1.84,0,0,0,1.3-1.49l2.23.35a2.24,2.24,0,0,0,1,0c.76-.23,1.06-1.12,1.16-1.91a1.33,1.33,0,0,0-.35-1.27,3.22,3.22,0,0,0,2.43-1.82,1.3,1.3,0,0,0,.13-.86c-.15-.49-.71-.71-1.19-.87a.55.55,0,0,1-.32-.19.52.52,0,0,1-.05-.28c0-.44.21-.87.19-1.31a1.86,1.86,0,0,0-1.53-1.55,5.19,5.19,0,0,0-2.31.18"" transform=""translate(-3.38 -5.54)"" fill=""#fff"" stroke=""#000"" stroke-miterlimit=""10""/></g>
	            <g><path id=""Head"" d=""M23.53,10.81c.11,0,0,0,.16.06.75.21,1.53.25,2.3.39a4,4,0,0,1,2.1.93.44.44,0,0,1,.12.15.43.43,0,0,1,0,.25,1.92,1.92,0,0,1-.41,1.1,1.72,1.72,0,0,1-1,.41,10,10,0,0,1-1.81.13,4.2,4.2,0,0,0-1,0"" transform=""translate(-3.38 -5.54)"" fill=""#fff"" stroke=""#000"" stroke-miterlimit=""10""/></g>
	            <g><path id=""Mark"" d=""M13.41,12.23a2.84,2.84,0,0,0-.71-.11,3.35,3.35,0,0,0-1.86.81"" transform=""translate(-3.38 -5.54)"" fill=""none"" stroke=""#000"" stroke-miterlimit=""10""/></g>
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(sheepSvg));
        var shape = sceneInfo.NodeIDs["Body"].Shapes[0];
        shape.Fill = new SolidFill() { Color = Color.white };
        shape = sceneInfo.NodeIDs["Head"].Shapes[0];
        shape.Fill = new SolidFill() { Color = Color.black };

        var tessOptions = new VectorUtils.TessellationOptions()
        {
            StepDistance = 100f,
            MaxCordDeviation = 0.5f,
            MaxTanAngleDeviation = 0.1f,
            SamplingStepSize = 0.01f
        };

        var geoms = VectorUtils.TessellateScene(sceneInfo.Scene, tessOptions);
        var sprite = VectorUtils.BuildSprite(geoms, 26, VectorUtils.Alignment.Center, Vector2.zero, 128, true);
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
