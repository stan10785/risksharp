<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RiskMap.aspx.cs" Inherits="Risk.RiskMap" %>

<html>
<head runat="server">
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no" />

    <script type="text/javascript" src="http://www.google.com/jsapi?key=ABQIAAAAGUxCV9iZQONy4Rve95BvxxT2yXp_ZAY8_ufC3CFXhHIE1NvwkxR6ik-sDf-jP2KTVQi5VSBF4ECGWA"></script>

    <script type="text/javascript">

        google.load("maps", "2");

        var poly1;

        function initialize() {

            var map = new google.maps.Map2(document.getElementById("map_canvas"));
            map.setCenter(new GLatLng(28, 10), 2);
            map.setMapType(G_PHYSICAL_MAP);

            var polygon = new GPolygon([
                    new GLatLng( 41.6, 41.77),
                    new GLatLng(38.6,48.31),
                    new GLatLng( 37.0,53.63             ),
                    new GLatLng( 36.6,61.13),
                    new GLatLng(26.4, 63.58),
                    new GLatLng(25.6, 61.8)
                    
                    
                  ], "#f33f00", 2, 1, "#ff0000", 0.2);
            map.addOverlay(polygon);
            poly1 = polygon;

            GEvent.addListener(polygon, 'click', function() {

                poly1.setFillStyle({ color: "#00FF00" });
                poly1.setStrokeStyle({ color: "#00FF00" });
            });

        }
      
    </script>

    <style type="text/css">
        #map_canvas
        {
            margin: auto auto;
            border: 1px solid black;
        }
    </style>
</head>
<body onload="">
    <form id="form1" runat="server">
    <div id="map_canvas" style="width: 1050; height: 550">
    </div>
    </form>
</body>
</html>
<!--
/*
            var bermudaTriangle;

            var triangleCoords = [
                new google.maps.LatLng(25.774252, -80.190262),
                new google.maps.LatLng(18.466465, -66.118292),
                new google.maps.LatLng(32.321384, -64.75737),
                new google.maps.LatLng(25.774252, -80.190262)
              ];

            // Construct the polygon
            // Note that we don't specify an array or arrays, but instead just
            // a simple array of LatLngs in the paths property
            bermudaTriangle = new google.maps.Polygon({
                paths: triangleCoords,
                strokeColor: "#FF0000",
                strokeOpacity: 0.8,
                strokeWeight: 2,
                fillColor: "#FF0000",
                fillOpacity: 0.35,
                title: "Bermuda Triangle"
            });


            google.maps.event.addListener(bermudaTriangle, 'click', function() {
                
            });

            map.addOverlay(bermudaTriangle);
            */
            -->
