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

This demo is not intended as the basis on which anything is to be built, but to show what is possible with design primitive data in the API. This application is not optimized in any way, nor is it a good implementation when it comes to OpenTK (OpenGL). Its intent is to purely demonstrate how to access this type of data in the Nexar API and what is possible.

## Prerequisites

Visual Studio 2019.

You need your Altium Live credentials and have to be a member of at least one Altium 365 workspace.

In addition, you need an application at [nexar.com] with the Design scope.
If you have not done this already, please [register at nexar.com](https://github.com/NexarDeveloper/nexar-forum/discussions/4).

Use the application client ID and secret and set environment variables `NEXAR_CLIENT_ID` and `NEXAR_CLIENT_SECRET`.

You will also need to set the environment variable `NEXAR_WORKSPACE_URL` to an existing Altium 365 workspace (e.g. `https://some-workspace.365.altium.com/`

## Known bugs / limitations

The only primitives that are currently supported are tracks, pads and vias. There are no component outlines, text, polygon pours, etc. It only supports top and bottom layers (and relies on those layers being named to contain such text).

There is also a known issue with some track segments missing. There is no support for arcs, including any tracks constructed from arcs.

These will be fixed in future but in alignment with priorities based on use cases, etc. - so, if you are working on something innovative and need additional things that are not currently supported, please reach out to us at support@nexar.com - we'd love to hear from you!

## How to use

Open the solution in Visual Studio.
Ensure `Nexar.Renderer` is the startup project, build, and run.

The browser is started with the identity server sign in page.
Enter your credentials and click `Sign In`.

The application window is activated and the workspace context automatically configured based on the aforementioned environment variable.

### Load a PCB project

Click on the `Open Project` menu option in the main window:

![image](https://user-images.githubusercontent.com/623551/193965439-9603ddd0-f35c-43cb-8828-7a9524a2d47c.png)

A dialog will open listing projects in the workspace along with thumbnails, names and descriptions. Click on the `Open` button on the project you want to open:

![image](https://user-images.githubusercontent.com/623551/193965636-3b9d444f-e429-4ec3-be24-97d674c0dea1.png)

After a few seconds (for larger designs this might take several seconds) you will see the PCB rendered. Some screenshots of different project renderings can be seen below.

There are a few keys supported for different actions:
- `R` - Reset view
- `W` - Zoom In
- `S` - Zoom out
- `Right Cursor` - Pan camera right
- `Left Cursor` - Pan camera left
- `Up Cursor` - Pan camera up
- `Down Cursor` - Pan camera down

There is a compile time constant `THREAD_PERIOD_MS` which controls the draw thread rate. It is currently set to 100ms to prevent larger designs from crashing due to performance issues.

![image](https://user-images.githubusercontent.com/623551/193965781-1b11d387-cd59-49a9-a95c-66a0c2b43162.png)

![image](https://user-images.githubusercontent.com/623551/193965795-885e6e54-3dd2-4eaa-bcc4-d1c4dc041ac1.png)

![image](https://user-images.githubusercontent.com/623551/193965804-eda77656-f482-44f8-82ee-e3c922efcf34.png)

![image](https://user-images.githubusercontent.com/623551/193965828-49a2e8ff-77f4-49ef-82b6-93c2fc00e4c3.png)

![image](https://user-images.githubusercontent.com/623551/193965839-38ffef31-0a13-4d4c-a88b-ca42b8783b54.png)
