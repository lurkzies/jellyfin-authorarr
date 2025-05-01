# [✒️] Authorarr

**Authorarr** is a simple metadata provider plugin for [Jellyfin](https://jellyfin.org) that fetches author images and basic details for book libraries.

It supports Open Library, and is designed to enhance author folders in your Jellyfin eBook collections with author portraits and brief biographies.

---

## [✨] Features

- Fetches author images from Open Library
- Populates author metadata like top work
- Designed for use with book libraries structured by author folder

---

## [📦] Installation

1. Build the plugin
    - Prerequisites: [.NET SDK 8.0.X](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
    - Clone this repository
    - Clone the [jellyfin/jellyfin](https://github.com/jellyfin/jellyfin) repository
    - Navigate to the cloned jellyfin repository and build the jellyfin server (if it isn't already built on your machine)
        `cd jellyfin ; dotnet build -c Release`
    - Clone the [jellyfin-plugin-template](https://github.com/jellyfin/jellyfin-plugin-template/) repository
    - Copy the Authorarr directory to the plugin template directory (`cp jellyfin-authorarr jellyfin-plugin-template/`)
    - Build the plugin! (`cd jellyfin-plugin-template/jellyfin-authorarr ; dotnet build -c Release`)

2. Install the plugin
    - Transfer the `.dll` and `manifest.json` to your Jellyfin server (the binary should be located in `jellyfin-plugin-template/jellyfin-authorarr/bin/Release/net8.0/Jellyfin.Plugin.Authorarr.dll`)
    - Move the files to `/jellyfin/config/plugins/Authorarr/`
        `mkdir -p /jellyfin/config/plugins/Authorarr ; cp Jellyfin.Plugin.Authorarr.dll /jellyfin/config/plugins/Authorarr/ ; cp manifest.json /jellyfin/config/plugins/Authorarr`
    - Restart Jellyfin!

---

## [⌨️] Usage

- Once the plugin is installed, you can update Author metadata by navigating to a book in the Jellyfin Web UI, and clicking the three dots, then selecting "Refresh Metadata" and using the default options.
- This should populate the Author's metadata with an image and a brief biography, along with some other basic information.

---

## [✅] To-Do List

- Fix the plugin configuration page
- Add a pre-built `.dll` to the repo
