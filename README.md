# LDtk Tileset Exporter
This aims to export individual files of tileset definitions from a project.  
This is made for LDtkToUnity, but it should also be usable in any other engine/framework.

## How to use
Execute from LDtk's custom command process. It's recommended to run after saving.  
![image](https://github.com/Cammin/LDtkTilesetExporter/assets/55564581/7f58061b-801d-4bae-9c6a-53bb302e1ea9)

## Note
This app also gathers all rectangles used in entity/level field instances for convenience. This supports both project & separate levels.  
because this additional data exists, this app writes an initial JSON object structured like this.  

`Rects` is an array of rectangles, and `Def` is the Tileset Definition:  
```json
{
    "Rects": []
    "Def": null
}
```

A rectangle is a simple object that is structured like this:
```json
{
    "x": 0,
    "y": 0,
    "w": 0,
    "h": 0,
}
```

This app uses [Utf8Json](https://github.com/neuecc/Utf8Json) to deserialize/serialize json quickly.
