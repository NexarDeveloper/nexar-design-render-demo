# Nexar.Renderer Demo

[nexar.com]: https://nexar.com/

Demo rendering primitives for an Altium 365 PCB design.

**Projects:**

- `Nexar.Renderer` - WinForms application that will load a project and render primitives (tracks, pads, vias) using OpenTK (OpenGL)
- `Nexar.Client` - GraphQL StrawberryShake client
- `OpenTK.WinForms` - OpenTK toolkit for OpenGL wrapper

## Intended Use Case

This demo is NOT intended as the basis for building an Altium 365 viewer. For this, we have a very complete embeddable viewer that is designed for a wide variety of use cases (it can be found here https://altium.com/viewer with example code on how to embed it here https://github.com/NexarDeveloper/altium365-embed-viewer).

This demo is intended for use cases that need to leverage PCB primitive information for the purposes of building capabilities that go beyond CAD viewing and collaboration. For example, this might be to create augmentation applications that see the physical and digital worlds combined, or downstream CAM type applications that would like to natively access design primitive data.

The API does not currently support geometric primitives, so this approach uses the PCB primitives and constructs the geometry based on primitive data, a line inflation algorithm (to create approximate geometric polygons) and then a tesselator to create the required triangle primitives for the shaders.

This demo is not intended as the basis on which anything is to be built, but to show what is possible with design primitive data in the API. This application is not optimized in any way, and it is a particularly basic implementation when it comes to OpenTK (OpenGL). **Its intention is to purely demonstrate how to access this type of data in the Nexar API and what is possible.**

## Prerequisites

Visual Studio 2019.

You need your Altium Live credentials and have to be a member of at least one Altium 365 workspace.

In addition, you need an application at [nexar.com] with the Design scope.
If you have not done this already, please [register at nexar.com](https://portal.nexar.com/sign-up).

Use the application client ID and secret and set environment variables `NEXAR_CLIENT_ID` and `NEXAR_CLIENT_SECRET`.


## Known bugs / limitations

The only primitives that are currently supported are tracks, pads, vias and component outlines. There is no support for text, polygon pours, etc. There is also no support for arcs, including any tracks constructed from arcs. It supports up to 16 layer designs with pre-configured layer colors. 

These will be fixed in future but in alignment with priorities based on use cases, etc. - so, if you are working on something innovative and need additional things that are not currently supported, please reach out to us at support@nexar.com - we'd love to hear from you!

## How to use

Open the solution in Visual Studio.
Ensure `Nexar.Renderer` is the startup project, build, and run.

The browser is started with the identity server sign in page.
Enter your credentials and click `Sign In`.

The application window is activated and the workspace context automatically configured based on the aforementioned environment variable.

### Load a PCB project

Click on the "Click to Load Workspaces" menu option in the main window:

![clickToLoadWorkspaces](https://user-images.githubusercontent.com/623551/221542274-85c66cbc-960c-4624-b8f0-82c6c288108c.png)

Select your workspace then click on the `Open Project` menu option:

![clickToOpenProject](https://user-images.githubusercontent.com/623551/221542178-dcd5628a-d484-433a-bf1c-4f51a10b083c.png)

A dialog will open listing projects in the workspace along with thumbnails, names and descriptions. Click on the `Open` button on the project you want to open:

![image](https://user-images.githubusercontent.com/623551/221542339-357e4d99-ec16-43d1-8378-764540d711a4.png)

After a few seconds (for larger designs this might take several seconds) you will see the PCB rendered. Any example of a large 16 layer design is shown below.

![image](https://user-images.githubusercontent.com/623551/221542462-661148b9-6042-41a4-bdd5-36ad2db3700a.png)

### Configure Visibility

You can control the layer visibility in the `Layers` menu option:

![image](https://user-images.githubusercontent.com/623551/221542591-9055f3bd-3189-4b7c-adad-964cf664ed4b.png)

The below example shows only two of the inner layers enabled.

![image](https://user-images.githubusercontent.com/623551/221544097-982a1c70-40c4-4e3e-97fa-0da7a8a1295b.png)

You can control the primitives that are visible from the `Primitives` menu option:

![image](https://user-images.githubusercontent.com/623551/221543217-5aabab55-4ecf-4151-ae2a-51a8bf9af5f5.png)

The below example shows only Pads and Component Outlines enabled.

![image](https://user-images.githubusercontent.com/623551/221543968-d4d8ed68-e1d7-44ac-b90d-4b565b57ed5c.png)

### Controls

There are a few keys supported for different actions:
- `R` - Reset view
- `W` - Zoom In
- `S` - Zoom out
- `N` - Pan camera and camera target left
- `M` - Pan camera and camera target right
- `I` - Pan camera and camera target up
- `J` - Pan camera and camera target down
- `Right Cursor` - Pan camera right (camera target unchanged)
- `Left Cursor` - Pan camera left (camera target unchanged)
- `Up Cursor` - Pan camera up (camera target unchanged)
- `Down Cursor` - Pan camera down (camera target unchanged)

Use the mouse wheel to zoom in / out. Right click and drag with the mouse to pan around the scene (moving the camera and camera target).

The below example shows the camera moved in the bottom left direction and the camra target moved to the bottom left quadrant of the PCB.

![image](https://user-images.githubusercontent.com/623551/221544367-91cf5daf-08c6-4a31-b841-28972d36d3fa.png)

### Comments

Comments are now supported. If comments exist in the design when it is opened, the comments panel will be shown auomatically on the right hand side. To enable / disable the panel and to refresh the comments (automatic refresh through GraphQL subscriptions is a work in progress) use the `Comments` menu option.

The below example shows the comments panel visible.

![image](https://user-images.githubusercontent.com/623551/221545138-93b27ff8-2977-47af-9222-ada436d35240.png)

Click on the comments in the comments panel to highlight them with a green box (comments are shown with a blue dotted line box when not selected)

Comments on area or on components are currently supported.

To create a comment on an area, left click and drag, release to finalize the area and a popup window will appear to enter the comment. Hit `Ctrl + Enter` or click `Create` to create the comment.

![image](https://user-images.githubusercontent.com/623551/221545768-3bfb04ae-e536-48d4-8a7d-5dcfdd9ef282.png)

To attach a comment to a component, right-click in the component outline and select the `Add Comment` option.

![image](https://user-images.githubusercontent.com/623551/221545999-b1870716-a78d-4151-8047-1f5ce9ad27b8.png)

Use the controls in each comment and comment thread in the comments panel to reply to a comment or delete or update existing commments.

![image](https://user-images.githubusercontent.com/623551/221546339-d9b121c7-2db3-49d9-83a5-fe43ed91fb07.png)
