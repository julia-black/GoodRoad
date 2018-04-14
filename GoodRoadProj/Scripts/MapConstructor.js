$(document).ready(function () {
    var myMap;
    var nameRout;
    var descriptionRout;
    var multiRoute;
    var myPlacemarks = [];
    var myPlacemark;

    ymaps.ready(init);

    function clickSaveRoute() {
        nameRout = $(".nameRoot").val();
        descriptionRout = $(".descriptionRout").val();

        var route = {
            type: 'FeatureCollection',
            features: []
        };
        for (let i = 0; i < myPlacemarks.length; i++) {
            route.features.push({
                type: 'Feature',
                geometry: {
                    type: 'Point',
                    coordinates: [
                    myPlacemarks[i].geometry.getCoordinates(0), myPlacemarks[i].geometry.getCoordinates(1),
                    ]
                },
                properties: {
                    balloonContent: '---',
                    hintContent: '-'
                },
                options: {
                    preset: 'islands#violetDotIcon'
                }
            });
        }

        if (myPlacemarks.length > 1) {
            var str = JSON.stringify(route);
            console.log(str);
            route = JSON.parse(str);
            console.log(route);
        }

        $.post("/Home/GetNewRoute", { routeData: str, routeName: nameRout, routeDiscription: descriptionRout }, function (data) {
            console.log(descriptionRoute);
        });
    }

    function addNewPlacemark() {
        var searchAddress = "Россия, Саратов";
        ymaps.geocode(searchAddress).then(
        function (res) {
            var firstGeoObject = res.geoObjects.get(0),
            coordinates = firstGeoObject.geometry.getCoordinates();

            console.log(res);
            myPlacemark = new ymaps.Placemark(coordinates, {}, { draggable: true });

            myPlacemarks.push(myPlacemark);
            console.log(myPlacemarks);

            myMap.geoObjects.add(myPlacemark);

            myPlacemark.events.add('dragend', function (e) {
                var placemarkCoordinates = e.get('target').geometry.getCoordinates();

                ymaps.geocode(placemarkCoordinates)
                .then(function (resAddress) {
                    // alert(placemarkCoordinates + ' ' + resAddress.geoObjects.get(0).properties.get('text')); 
                });

                if (myPlacemarks.length > 1) {
                    if (multiRoute) {
                        myMap.geoObjects.remove(multiRoute);
                    }
                    var placemarks = [];
                    for (let i = 0; i < myPlacemarks.length; i++) {
                        placemarks.push(myPlacemarks[i].geometry.getCoordinates());
                    }
                    //alert(placemarks);

                    multiRoute = new ymaps.multiRouter.MultiRoute({
                        referencePoints: placemarks,
                        params: {
                            routingMode: 'pedestrian'
                        }
                    }, {
                        editorDrawOver: false,
                        wayPointDraggable: true,
                        viaPointDraggable: true,
                        routeStrokeColor: "000088",
                        routeActiveStrokeColor: "ff0000",
                        pinIconFillColor: "ff0000",
                        boundsAutoApply: true,
                        zoomMargin: 30
                    });
                    myMap.geoObjects.remove(myPlacemarks);
                    myMap.geoObjects.add(multiRoute);
                    myMap.geoObjects.remove(myPlacemark);
                }
            });
            console.log(multiRoute);
        }
        );
    }

    function clickResetRoute() {
        console.log("reset");
        myMap.geoObjects.removeAll();
    }

    function init() {
        myMap = new ymaps.Map("map", {
            center: [51.533970, 46.021121],
            zoom: 12
        });

        $(".addElemButton").on('click', addNewPlacemark);
        $(".saveButton").on('click', clickSaveRoute);
        $(".resetButton").on('click', clickResetRoute);
    }
});