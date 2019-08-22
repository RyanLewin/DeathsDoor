using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VectorGraphics;

public class Appearance : MonoBehaviour
{
    WorldController worldController;
    Citizen citizen;
    Color skinTone;
    Color hairColour;

    [SerializeField]
    Transform[] hands = default;

    Color ChangeColorBrightness (Color color, float diff = .2f)
    {
        float correctionFactor = Random.Range(1 - diff, 1 + diff);
        
        Color c = color * correctionFactor;
        c.a = 1;
        return c;
    }

    public void SetAppearance ()
    {
        worldController = WorldController.GetWorldController;
        citizen = GetComponent<Citizen>();

        skinTone = worldController.skinTones[Random.Range(0, worldController.skinTones.Length)];
        skinTone = ChangeColorBrightness(skinTone);

        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 30.11 27.39"">
                <g><path id=""Body"" d=""M30.81,17.36c0,7.43-6.63,13.45-14.81,13.45s-14.81-6-14.81-13.45a12.56,12.56,0,0,1,.36-3C3,8.4,8.94,3.92,16,3.92,24.18,3.92,30.81,9.94,30.81,17.36Z"" transform=""translate(-0.94 -3.67)"" fill=""#fff"" stroke=""#000"" stroke-miterlimit=""10"" stroke-width=""0.5""/></g>";
        
        Hand(0);
        Hand(1);

        int hairChance = Random.Range(0, citizen.gender ? 100 : 55);
        SVGParser.SceneInfo sceneInfo = default;
        if (hairChance < 70)
        {
            SetHair(ref svg, ref sceneInfo);
        }
        else
        {
            svg += "</svg>";

            sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        }

        var shape = sceneInfo.NodeIDs["Body"].Shapes[0];
        shape.Fill = new SolidFill() { Color = skinTone };
        
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

    void Hand (int hand)
    {
        string svg =
            @"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 30.11 27.39"">
                <g><circle id=""Hand"" cx=""4.66"" cy=""2.6"" r=""2.11"" fill=""#fff"" stroke=""#000"" stroke-miterlimit=""10"" stroke-width=""0.5""/></g>
            </svg>";
        
        var sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var shape = sceneInfo.NodeIDs["Hand"].Shapes[0];
        shape.Fill = new SolidFill() { Color = skinTone };

        var tessOptions = new VectorUtils.TessellationOptions()
        {
            StepDistance = 100f,
            MaxCordDeviation = 0.5f,
            MaxTanAngleDeviation = 0.1f,
            SamplingStepSize = 0.01f
        };

        var geoms = VectorUtils.TessellateScene(sceneInfo.Scene, tessOptions);
        var sprite = VectorUtils.BuildSprite(geoms, 26, VectorUtils.Alignment.Center, Vector2.zero, 128, true);
        hands[hand].GetComponent<SpriteRenderer>().sprite = sprite;
    }

    void SetHair (ref string svg, ref SVGParser.SceneInfo sceneInfo)
    {
        if (citizen.age >= 55)
            hairColour = Color.grey;
        else
            hairColour = worldController.hairColours[Random.Range(0, worldController.hairColours.Length)];
        hairColour = ChangeColorBrightness(hairColour);

        //string hairSvg =
        //    @"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 30.21 14.6"">";

        int style = 0;
        if (!citizen.gender)
        {
            RandomHairStyle(ref svg, out style);
        }
        svg += @"<g><path id=""Hair1"" d=""M30.82,16.71c.29,7.6-6.42,14-14.57,14.1-8.33.12-15.34-6.33-15-14.1"" transform=""translate(-1 -2.6)"" fill=""#fff"" stroke=""#040000"" stroke-linecap=""round"" stroke-miterlimit=""10"" stroke-width=""0.5""/></g>
                        <g><path id=""Hair2"" d=""M1.12,16.71a29.34,29.34,0,0,0,14.88,4,29.25,29.25,0,0,0,14.75-4"" transform=""translate(-1 -2.6)"" fill=""#fff"" stroke=""#000"" stroke-linecap=""round"" stroke-miterlimit=""10"" stroke-width=""0.5""/></g>
                        </svg>";
        sceneInfo = SVGParser.ImportSVG(new StringReader(svg));
        var hairShape = sceneInfo.NodeIDs["Hair1"].Shapes[0];
        hairShape.Fill = new SolidFill() { Color = hairColour };
        hairShape = sceneInfo.NodeIDs["Hair2"].Shapes[0];
        hairShape.Fill = new SolidFill() { Color = skinTone };
        if (!citizen.gender)
        {
            switch (style)
            {
                case 0:
                    hairShape = sceneInfo.NodeIDs["PonyTail"].Shapes[0];
                    hairShape.Fill = new SolidFill() { Color = hairColour };
                    hairShape = sceneInfo.NodeIDs["PonyTail2"].Shapes[0];
                    hairShape.Fill = new SolidFill() { Color = hairColour };
                    break;
                case 1:
                    hairShape = sceneInfo.NodeIDs["PonyTail"].Shapes[0];
                    hairShape.Fill = new SolidFill() { Color = hairColour };
                    break;
            }
        }

        //var tessOptions = new VectorUtils.TessellationOptions()
        //{
        //    StepDistance = 100f,
        //    MaxCordDeviation = 0.5f,
        //    MaxTanAngleDeviation = 0.1f,
        //    SamplingStepSize = 0.01f
        //};

        //var geoms = VectorUtils.TessellateScene(hairSceneInfo.Scene, tessOptions);
        //var sprite = VectorUtils.BuildSprite(geoms, 26, VectorUtils.Alignment.Center, Vector2.zero, 128, true);
        //GameObject go = new GameObject("Hair");
        //go.transform.SetParent(transform);
        //transform.GetChild(0).gameObject.SetActive(true);
        //go.AddComponent<SpriteRenderer>();
        //go.transform.localPosition = new Vector3(0, citizen.gender ? -.25f : -.355f, 0);
        //SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        //sr.sprite = sprite;
        //sr.sortingLayerID = GetComponent<SpriteRenderer>().sortingLayerID;
        //sr.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
    }

    void RandomHairStyle (ref string hairSvg, out int style)
    {
        style = Random.Range(0, 2);
        switch (style)
        {
            case 0:
                hairSvg += @"<g><path id=""PonyTail"" d=""M14.55,31.8c-.15.38-.26.39-.36.69a1.22,1.22,0,0,0,0,.81,1.62,1.62,0,0,0,.57.67,2.22,2.22,0,0,1,.49.64,2.06,2.06,0,0,1,0,1.78c.35.21.74-.12,1-.43a6.62,6.62,0,0,0,1.05-1.34,5.23,5.23,0,0,0,.6-2.42,1.57,1.57,0,0,0-.08-.49,1.1,1.1,0,0,0-.34-.4,2.69,2.69,0,0,0-1.6-.5c-.33,0-.92,0-1.15.41C14.62,31.38,14.68,31.45,14.55,31.8Z"" transform=""translate(7 -4.6)"" fill=""red""/>
                    <path d=""M14.29,31.64c-.27.56-.63,1.08-.41,1.73a1.58,1.58,0,0,0,.43.62c.62.69,1,1.27.63,2.25a.3.3,0,0,0,.18.44c1.06.25,2-1.1,2.43-1.91s.95-2.3.52-3.21a2.41,2.41,0,0,0-2.16-1,1.45,1.45,0,0,0-1.65,1.21c-.11.37.46.52.58.15a1,1,0,0,1,1.07-.76,1.56,1.56,0,0,1,1.68,1.09,3.75,3.75,0,0,1-.43,2c-.21.48-1.17,2.09-1.88,1.93l.18.44a2.46,2.46,0,0,0,.06-2,2.27,2.27,0,0,0-.39-.58c-.34-.39-.39-.33-.49-1s-.08-.53.17-1C15,31.6,14.46,31.3,14.29,31.64Z"" transform=""translate(7 -4.6)""/></g>";
                hairSvg += @"<g><path id=""PonyTail2"" d=""M14.55,31.8c-.15.38-.26.39-.36.69a1.22,1.22,0,0,0,0,.81,1.62,1.62,0,0,0,.57.67,2.22,2.22,0,0,1,.49.64,2.06,2.06,0,0,1,0,1.78c.35.21.74-.12,1-.43a6.62,6.62,0,0,0,1.05-1.34,5.23,5.23,0,0,0,.6-2.42,1.57,1.57,0,0,0-.08-.49,1.1,1.1,0,0,0-.34-.4,2.69,2.69,0,0,0-1.6-.5c-.33,0-.92,0-1.15.41C14.62,31.38,14.68,31.45,14.55,31.8Z"" transform=""translate(-9 -4.6)"" fill=""red""/>
                    <path d=""M14.29,31.64c-.27.56-.63,1.08-.41,1.73a1.58,1.58,0,0,0,.43.62c.62.69,1,1.27.63,2.25a.3.3,0,0,0,.18.44c1.06.25,2-1.1,2.43-1.91s.95-2.3.52-3.21a2.41,2.41,0,0,0-2.16-1,1.45,1.45,0,0,0-1.65,1.21c-.11.37.46.52.58.15a1,1,0,0,1,1.07-.76,1.56,1.56,0,0,1,1.68,1.09,3.75,3.75,0,0,1-.43,2c-.21.48-1.17,2.09-1.88,1.93l.18.44a2.46,2.46,0,0,0,.06-2,2.27,2.27,0,0,0-.39-.58c-.34-.39-.39-.33-.49-1s-.08-.53.17-1C15,31.6,14.46,31.3,14.29,31.64Z"" transform=""translate(-9 -4.6)""/></g>";
                break;
            case 1:
                hairSvg += @"<g><path id=""PonyTail"" d=""M14.55,31.8c-.15.38-.26.39-.36.69a1.22,1.22,0,0,0,0,.81,1.62,1.62,0,0,0,.57.67,2.22,2.22,0,0,1,.49.64,2.06,2.06,0,0,1,0,1.78c.35.21.74-.12,1-.43a6.62,6.62,0,0,0,1.05-1.34,5.23,5.23,0,0,0,.6-2.42,1.57,1.57,0,0,0-.08-.49,1.1,1.1,0,0,0-.34-.4,2.69,2.69,0,0,0-1.6-.5c-.33,0-.92,0-1.15.41C14.62,31.38,14.68,31.45,14.55,31.8Z"" transform=""translate(-1 -1.9)"" fill=""red""/>
                    <path d=""M14.29,31.64c-.27.56-.63,1.08-.41,1.73a1.58,1.58,0,0,0,.43.62c.62.69,1,1.27.63,2.25a.3.3,0,0,0,.18.44c1.06.25,2-1.1,2.43-1.91s.95-2.3.52-3.21a2.41,2.41,0,0,0-2.16-1,1.45,1.45,0,0,0-1.65,1.21c-.11.37.46.52.58.15a1,1,0,0,1,1.07-.76,1.56,1.56,0,0,1,1.68,1.09,3.75,3.75,0,0,1-.43,2c-.21.48-1.17,2.09-1.88,1.93l.18.44a2.46,2.46,0,0,0,.06-2,2.27,2.27,0,0,0-.39-.58c-.34-.39-.39-.33-.49-1s-.08-.53.17-1C15,31.6,14.46,31.3,14.29,31.64Z"" transform=""translate(-1 -1.9)""/></g>";
                break;
        }
    }
}
