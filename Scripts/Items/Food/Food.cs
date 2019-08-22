using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VectorGraphics;

public class Food : Item
{
    public FoodType type;

    // Start is called before the first frame update
    void Start()
    {
        SetAppearance();
    }

    void Used (Citizen citizen)
    {
        count -= 1;
        if (count <= 0)
        {
            RanOut();
        }
    }

    void SetAppearance ()
    {
        if (type == FoodType.Crisps)
        {
            SetCrisps();
        }
    }

    void SetCrisps ()
    {
        string crisp =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 11.44 13.95"">
                <g><path id=""Packet"" d=""M13.13,13.9H3.39a5.9,5.9,0,0,1-.81-3.15c0-1.51.75-2.34.63-4.08a16.67,16.67,0,0,0-.39-1.79A5.66,5.66,0,0,1,3.21,1L12.6,1a15.62,15.62,0,0,1,.33,5c-.19,1.94-.78,2.25-.56,3.63.2,1.21.79,2.12.53,3.63a4.26,4.26,0,0,1-.27.93"" transform=""translate(-2.07 -0.47)"" fill=""none"" stroke=""#000"" stroke-miterlimit=""10""/></g>
                <g><text transform=""translate(2.33 5.11) rotate(-7.27)"" font-size=""2.5"">B</text></g>
                <g><text transform=""translate(3.66 5) rotate(-2.06)"" font-size=""2.5"" font-family=""ArialNarrow-Italic, Arial"">I</text></g>
                <g><text transform=""translate(4.23 5.03) rotate(0.56)"" font-size=""2.5"" font-family=""ArialNarrow-Italic, Arial"">T</text></g>
                <g><text transform=""translate(5.45 5.07) rotate(2.51)"" font-size=""2.5"" font-family=""ArialNarrow-Italic, Arial"">E</text></g>
                <g><text transform=""translate(6.66 5.17) rotate(3.93)"" font-size=""2.5"" font-family=""ArialNarrow-Italic, Arial"">R</text></g>
                <g><text transform=""translate(8.07 5.27) rotate(0.35)"" font-size=""2.5"" font-family=""ArialNarrow-Italic, Arial"">S</text></g>
                <g><ellipse id=""Crisp1"" cx=""8.34"" cy=""9.75"" rx=""2.04"" ry=""1.5"" transform=""translate(-5.83 5) rotate(-30)"" fill=""#ffdc7f"" stroke=""#000"" stroke-miterlimit=""10"" stroke-width=""0.15""/></g>
                <g><ellipse id=""Crisp0"" cx=""6.77"" cy=""8.95"" rx=""2.04"" ry=""1.5"" transform=""translate(-6.44 9.86) rotate(-60)"" fill=""#ffdc7f"" stroke=""#000"" stroke-miterlimit=""10"" stroke-width=""0.15""/></g>
            </svg>";

        var sceneInfo = SVGParser.ImportSVG(new StringReader(crisp));
        var shape = sceneInfo.NodeIDs["Packet"].Shapes[0];
        shape.Fill = new SolidFill() { Color = Color.red };

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

public enum FoodType { Crisps, TinCan }
