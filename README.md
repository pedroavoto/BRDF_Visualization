# Rendering Framework for BRDF Visualization

### INTRODUCTION

A Bi-directional Reflectance Distribution Function (BRDF) is a type of mathematical function that illustrates how light is reflected when it hits a surface. Major characteristics of BRDF are symmetry between incident and reflected directions and that the total reflected power of incident radiation should be less than or equal to the energy of the incident light for any given direction. Ambient term, diffuse term and the specular term are the main contributions of a lighting model. The ambient term is often called indirect light. It is the light that bounces in other objects before hitting the eye. The proper implementation of an ambient term is not in the scope of the project, this will be simulated through a constant ambient factor. This project in particular, will focus on the diffuse term and the specular term.  The diffuse term is also referred to as local subsurface scattering. It models the contribution of light being scattered inside the object and then being re-emitted from the surface. A specular highlight is that bright spot of light visible on shiny objects when it is hit by light. It provides powerful visual signs towards the shape of an object and its location in the scene, which are crucial elements in 3D computer graphics. 

The project basically analyze the emissions of a ray of light in an object known, on this case a bunny got from the open source models [4], when hit the body of the element and reflected.

The application (BRDF) used DirectX 11 as the graphic API (SharpDX wrapper of DirectX 11) and the AntTweakBar[5] User Interface Library to create a light direction, the buttons to select color, the BRDF models and the roughness of the object. 

### PIPELINE GRAPHIC

The rendering pipeline main goal is to take a group of 3D object descriptions and convert them into an applicable image format for visualization in the output window of an application. The figure 01 below shows a full pipeline graphic used to implement this project in particular.

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62652498-62388f80-b910-11e9-8f40-42fe9adc7029.png">
</p>
<p align="center">
  Figure 01 - Pipeline Graphic BRDF Visualization Application
</p>

Pipeline stages are represented in different colors (green and blue). Blue gives us the programmable stages and green provides the fixed-function stages. The pipeline graphic entry point is the input assembler stage. This stage reads the input data from resources and then assembles vertices for later use. This lets us leverage multiple vertex buffers and allows instanced rendering to be used. The connectivity of the vertices is also determined according to the the input resources and the rendering configuration. Both the primitive connectivity and the assembled vertices data are transferred down to the pipeline. 

The vertex shader stage obtains and interprets the assembled vertex data from the input assembler stage to process a single vertex at a time. This is the first programmable stage in the pipeline, applying the current vertex shader program to each input vertex. The main job of the vertex shader is to project the vertex position into the clip space. Generally, operations that have to be executed on each vertex of the input model should be run in the vertex shader. 

The object projected is rasterized and dealt with at the rasterized level after this point. The rasterized level is a data group that corresponds to a pixel in a render target, and that can be leveraged to update the current value of that pixel once it makes its way through the pipeline. The rasterizer also provides a depth value for each fragment, this can be later used for visibility testing during the output merger stage.

With a rasterized stage generated, the pixel shader is then called to process it. The pixel shader is needed to create a color value output for each one of the render target outputs that are part of the pipeline. In order to do this, it performs computations on the incoming fragment attributes.In addition, It can also modify the depth value created by the rasterizer stage, which lets specialized algorithms to be added in the pixel shader stage. 

Once the pixel shader has completed its tasks, its results are then moved to the output merger stage. The output merger has to merge the pixel shader output with the bound depth and the render target resources correctly. This includes performing blending functions, running depth tests to write the output.

### BRDF MODELS

At this section, I am going to present all the models that I used in my BRDF application. Once again, the models listed below were implemented at the pixel shader stage, more details see the document Lightning.hlsl. The following models are given different inputs but have as objective to calculate the result of the color that will be drawn in each pixel.

#### Lambert Diffuse Model

Lambertian reflection is often used as a model for diffuse reflection, and because it is easy to implement and it is cheaper when I talk about processing data, I chose this model. Basically, The reflection is calculated by taking the dot product of the normal vector (normalized) with light source vector (normalized). Following the lambert law, the dot product between those vectors is the same as the cosine of the angle between those ones. As I can see, the dot product between those angles varies from 0 to 1. If the light direction is at the same direction as the normal vector more ray of lights are hitting the object and more lighted that part is going to be. Contrary, If the angle between those vector is 90 degree, less light will be hit the object and according to the lambert law, the cosine of the angle will be lower (tend to 0). In other words, more dark the piece of the object will be. The figure 02 and 03 below show us how this spectrum changes toward the cosine of the angle.

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62652874-2f42cb80-b911-11e9-812f-bd8431c54c63.png">
</p>
<p align="center">
  Figure 02 - Lambert BRDF
</p>

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62652913-3e297e00-b911-11e9-8678-6fcb029cf4d1.png">
</p>
<p align="center">
  Figure 03 - Cosine Law
</p>

As I mention at the pipeline graphic section, the BRDF models will be implement at the pixel shader stage to evaluate with color each pixel will be “painted”. The HLSL (GPU code) reads as follows:

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62652969-69ac6880-b911-11e9-911a-de17378b6d5c.png">
</p>
<p align="center">
  Figure 04 - Lambert HLSL code
</p>

#### Blinn-Phong Specular Model

Now, I am going to talk about the first specular model implemented in our application, Blinn-Phong Specular Model. This model is still very often used in commercial games, and it is cheaper to computationally functions. The reason to be a cheaper model is because instead of use reflectance vector to calculate the specular term (phong method), Blinn suggested to use a half vector between the light direction and camera vector. Considering that the angle between the half vector and the surface normal is smaller than the angle between reflected and camera vector used in Phong's model, Blinn-phong model provides less expensive computation processing.

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653066-a0827e80-b911-11e9-842f-7f2ddc6ce13c.png">
</p>
<p align="center">
  Figure 05
</p>

As we can see at the proposed Blinn-phong formula, we have a exponent term applied in a dot product between the half vector and the camera vector. This exponent term in the blinn-phong model is important to determine how broad the specular highlight is going to be and helps to a smoothly changes transition between the white light spot and the the color of the object. The figure below show us how tighter is the specular reflection luminous when we vary the exponent term.

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653120-bb54f300-b911-11e9-9909-18f836d7a267.png">
</p>
<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653125-bee87a00-b911-11e9-8185-e4f3a5d4e97f.png">
</p>
<p align="center">
  Figure 06 - Formula Blinn-Phong and Half vector
</p>

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653187-e2132980-b911-11e9-907d-72ec4ab2cff6.png">
</p>
<p align="center">
  Figure 07 - Exponent Term
</p>

This model considering 4 argument: the light direction and the camera vector to compute the half vector, the normal vector and the position of the object in the world. The HLSL (GPU code) reads as follows:

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653259-0a028d00-b912-11e9-802c-414837661a4e.png">
</p>
<p align="center">
  Figure 08 - Blinn-Phong HLSL code
</p>

#### Oren Nayar Diffuse Model

Oren-Nayar model were proposed by Michael Oren and Shree K. Nayar,  and the purpose of this model is measure how the reflectance works in a rough diffuse surfaces. The Oren-Nayar model is the microfacet model which assumes the surface to be composed of long symmetric cavities [3]. The model consider accounts for masking and shadowing, microfacet distribution along the object. The distribution shown at the formulas below, provides very good outputs as an approximation of the microfacet distribution.

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653294-20104d80-b912-11e9-8e16-578bff9f2c14.png">
</p>
<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653297-23a3d480-b912-11e9-8c21-90e7233595e2.png">
</p>
<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653301-269ec500-b912-11e9-80a2-18c7f3222658.png">
</p>
<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653306-2acae280-b912-11e9-839c-55fdafe95514.png">
</p>
<p align="center">
  Figure 09 - Distributions term - Roughness (σ)
</p>

The Oren-Nayar diffuse model can be viewed as a generalization of Lambert’s law. As we can see at the formula below, if the roughness of the body has zero value, the term A and B for the distribution consideration will be 1 and 0 respectively, consequently proving the statement above.

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653351-3dddb280-b912-11e9-9a6d-accc59a66f21.png">
</p>
<p align="center">
  Figure 10 - Standard formula of Oren-Nayar Model
</p>

According to the pipeline graphic section, the BRDF models will be implement at the pixel shader stage to evaluate with color each pixel will be “painted”. The HLSL (GPU code) reads as follows:

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653420-6665ac80-b912-11e9-9f4c-e2a155125c2c.png">
</p>
<p align="center">
  Figure 11 - Oren-Nayar HLSL code
</p>

#### Cook-Torrance Specular Model

The Cook-Torrance model is the second specular model that I implemented at my BRDF project. This model has the same approach as the Oren-Nayar model, considering the microfacet model which assumes the surface to be composed of long symmetric cavities [3]. This model is compounded by three main parts that will be listed below.

##### Distribution Function

The distribution function is a algorithms that consider the roughness of the material, adapting the orientation of the microfacet as a statistical function. Even though the literature shows a lot of different functions, I decided to use the GGX distribution function described below:

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653480-84cba800-b912-11e9-831f-344feb27bf20.png">
</p>
<p align="center">
  Figure 12 - Microfacet distributions
</p>

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653503-8f863d00-b912-11e9-8906-acfe91e44966.png">
</p>
<p align="center">
  Figure 13 - Distribution Function
</p>

##### Geometry Function

The geometry function is another statistical function that also consider the roughness of the material but it focus specifically in the attenuation of the light. The image below represent the interaction of the ray of the light between the microfacet, and show us how the masking and the shadowing effect the reflectance of the light. Therefore, the geometry function is going to evaluate also the part that light does not hit the microfacet. Even though the literature shows a lot of different functions, I decided to use the GGX geometry function described below:

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653560-a75dc100-b912-11e9-80d2-13577e794b44.png">
</p>
<p align="center">
  Figure 14 - Shadowing and Masking
</p>

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653597-b04e9280-b912-11e9-95f3-4356ac0ee8ef.png">
</p>
<p align="center">
  Figure 15 - Geometry Function
</p>

##### Fresnel Function

Basically, the fresnel effect describes the relationship between the angle that you perceive at a surface and the amount of reflectivity you see [2]. As we can see at the figure 16, if the angle of the view direction is parallel to the object, the object becomes much closer to a mirror. In project I applied the Shlick model because is easy to implement, gives greater control of the appearance of the material, and seemed appropriate to use in real time shaders.

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653640-ca887080-b912-11e9-97f7-3905584fb7d4.png">
</p>
<p align="center">
  Figure 16 - Example of Fresnel
</p>

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653653-d4aa6f00-b912-11e9-908f-ac33ae401b97.png">
</p>
<p align="center">
  Figure 17 - Fresnel Formula
</p>

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653677-e9870280-b912-11e9-8745-fb209675604b.png">
</p>
<p align="center">
  Figure 18 - Distribution, Geometry and Fresnel HLSL code
</p>

Finally, adding all the terms listed below we can calculate the cook-torrance lightning model for the specular contribution.

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653744-0de2df00-b913-11e9-8bcd-638f475969e2.png">
</p>
<p align="center">
  Figure 19 - Standard Formula Cook-Torrance
</p>

<p align="center">
  <img src="https://user-images.githubusercontent.com/49216807/62653751-163b1a00-b913-11e9-822f-f1dfee1af249.png">
</p>
<p align="center">
  Figure 20 - Cook-Torrance HLSL code
</p>

### RESULT

#### Lambert Diffuse Model

![image](https://user-images.githubusercontent.com/49216807/62653802-336fe880-b913-11e9-97f7-afbc34fb6d0c.png)

#### Blinn-Phong Specular Model

![image](https://user-images.githubusercontent.com/49216807/62653828-484c7c00-b913-11e9-906d-32b831e414a1.png)

#### Oren-Nayar Diffuse Model

<p float="left">
  <img src="https://user-images.githubusercontent.com/49216807/62653853-58fcf200-b913-11e9-88c2-ab6835b56d2d.png" />
  <img src="https://user-images.githubusercontent.com/49216807/62653858-5bf7e280-b913-11e9-8292-5e5d3308f7b3.png" /> 
  <img src="https://user-images.githubusercontent.com/49216807/62653876-6b772b80-b913-11e9-8280-608ed778cfc5.png" />
</p>

#### Cook-Torrance Specular Model

<p float="left">
  <img src="https://user-images.githubusercontent.com/49216807/62653899-7a5dde00-b913-11e9-90af-b1cbddfe32f2.png" />
  <img src="https://user-images.githubusercontent.com/49216807/62653904-82b61900-b913-11e9-9937-97947e8e87cf.png" /> 
  <img src="https://user-images.githubusercontent.com/49216807/62653913-877acd00-b913-11e9-9420-04abf82a73ce.png" />
  <img src="https://user-images.githubusercontent.com/49216807/62653921-8d70ae00-b913-11e9-81bd-8a54a1de93ae.png" />
</p>


### CONCLUSION


The main goal of the project was to code software able to visualize different BRDF models. One feature that was not explored was the addition of ambient term. This term includes the light that enters an environment and bounces several times around it before lighting any object and it could be added in future iterations.

On a personal level, I selected this project because the area of computer graphics has been an interest of mine since I was young. I have always being fascinated by the graphics of games and movies. In addition, I also wanted to learn more about DirectX, C# and SharpDX and through this project I was able to learn and develop skills that can help me thrive in this field.


### REFERENCE


[1] Jason Zink , Matt Pettineo , Jack Hoxley, Practical Rendering & Computation with Direct3D 11, 1st A. K. Peters, Ltd. Natick, MA, USA 2011.

[2] Tomas Akenine-Möller, Eric Haines, Naty Hoffman, Real-Time Rendering, A.K. Peters Ltd., 3rd edition, 2008.

[3] https://en.wikipedia.org/wiki/Oren%E2%80%93Nayar_reflectance_model - Oren-Nayar Model.

[4] http://www.prinmath.com/csci5229/OBJ/index.html - Bunny Object format .obj.

[5] http://anttweakbar.sourceforge.net/doc/ - UI Library.





