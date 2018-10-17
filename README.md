# Spline Tools #

This repo represents a set of tools to generate representation of splines and curves to render within Unity. Currently, the 
tool can generate and store a collection of sampled points along a CatmullRomSpline.

More spline types are to come!

## QuickStart ##
Simply attach a `PointsContainer` to any gameObject and create points.

![Inspector](https://github.com/psuong/SplineTools/blob/overhaul/structure/Images/PointsContainer.png)

### Fields ###
* Type:         The kind of spline to generate
* LineSegments: How many segments should each subspline be splitted into?
* HandlesSize:  The size of the movement handles within the Scene
* HeightClamp:  What should the heights of all spline be?
* LineSpacing:  The far should the space between each line segment be for the preview?
* HandlesColor: The color of the handles.
* LineColor:    The color of the preview lines.
* NormalColor:  The color of the preview normals.
* Points:       The series of control points to generated the spline.

### SceneView ###
![Scene](https://github.com/psuong/SplineTools/blob/overhaul/structure/Images/CatmullRomPreview.png)

## Dependencies ##
* Unity
* C# 7
* .NET4.6

## Notes ##
This is an experimental repo for me to explore so expect optimizations and refactors. I do use C# 7, so Unity should be targetted 
on the .NET 4.6 runtime instead of 3.5.

### Potential Uses ###
* Road generation/previews

### Limitations ###
Only looped CatmullRom Splines have array buffers which are properly calculated. If you want to write to a buffer for non looped 
CatmullRom Splines, use an `IList<Vector3>` instead. You should have enough points to generate the splines.

## Immediate Goals ##
* [ ] Tests to check the math of creating buffers
* [ ] Safety checking
* [ ] Burst compiler ready

## Potential Future Goals ##
* [ ] Procedural mesh generation
* [ ] Editor Windows
