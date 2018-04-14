$(document).ready(function () {
    var myMap;
    var allRoutes = [];
    var routes = [];
    var myPlacemarks;
    var chooseRoutes = 0;
    var geolocation;
    var myGeolocation;
    var jsonGeo;
    var balloonLayout;
    // var closes 
    var arrColors = ["#00FF00", "#FF0000", "#20B2AA", "#FF1493", "#C71585", "#0000FF", "#4B0082", "#800000", "#0000FF", "#800080", "#FF4500", "#006400"];

    //-2 - ближайшие 
    // 0 - все 
    // -1 - мои 
    // 1,2,3 - id 

    ymaps.ready(init);

    $("#allRoutes").on('click', setChooseRoutesAll);
    $("#myRoutes").on('click', setChooseRoutesMy);
    $("#closestRoutes").on('click', setChooseRoutesClosest);


    $("[name='GetRouteByID']").on("click", function () {
        let id = $(this).attr("id");
        chooseRoutes = id;
        getAllRoutes();
    });

    function init() {
        geolocation = ymaps.geolocation;

        myMap = new ymaps.Map("map", {
            center: [51.533970, 46.021121],
            zoom: 12
        });

        balloonLayout = ymaps.templateLayoutFactory.createClass(
        "<div class='my-balloon'>" +
        '<a class="close" href="#">&times;</a>' +
        "<b>Маршрут {% if properties.type == 'driving' %}" +
        "на автомобиле<br/>" +
        "{% else %}" +
        "на общественном транспорте" +
        "{% endif %}</b><br />" +
        "Расстояние: " +
        "<i>{{ properties.distance.text }}</i>,<br />" +
        "Время в пути: " +
        "<i>{{ properties.duration.text }} (без учета пробок) </i>" +
        "</div>", {

            build: function () {
                this.constructor.superclass.build.call(this);
                this._$element = $('.my-balloon', this.getParentElement());
                this._$element.find('.close')
                .on('click', $.proxy(this.onCloseClick, this));
            },

            onCloseClick: function (e) {
                e.preventDefault();
                this.events.fire('userclose');
            }
        }
        );

        geolocation.get({
            provider: 'browser',
        }).then(function (result) {
            result.geoObjects.options.set('preset', 'islands#blueCircleIcon');
            myGeolocation = result.geoObjects;
            console.log("geo");
            console.log(myGeolocation);
            myMap.geoObjects.add(myGeolocation);

            jsonGeo = JSON.stringify(myGeolocation.position);
            console.log("jsonGeo");
            console.log(myGeolocation.position);
            console.log(jsonGeo);

            // $.post("/Home/GetClosestRoutes", { positionData: jsonGeo }, function (data) { 
            // console.log("post"); 
            // console.log(data); 
            // 
            // allRoutes = JSON.parse(data); 
            // console.log(allRoutes); 
            // setRoutes(); 
            // }); 

        });
        getAllRoutes();
    }

    function setChooseRoutesAll() {
        chooseRoutes = 0;
        getAllRoutes();
    }

    function setChooseRoutesMy() {
        chooseRoutes = -1;
        getAllRoutes();
    }

    function setChooseRoutesClosest() {
        chooseRoutes = -2;
        getAllRoutes();
    }

    function getAllRoutes() {
        //все маршруты 
        if (chooseRoutes == 0) {
            console.log("0");
            clearMap();
            // myMap.geoObjects.add(myGeolocation); 
            $.post("/Home/GetRoutes", null, function (data) {
                allRoutes = JSON.parse(data);
                console.log(allRoutes);
                setRoutes();
            });
        }
        else if (chooseRoutes == -1) {
            console.log("-1");
            clearMap();
            myMap.geoObjects.add(myGeolocation);
            $.post("/Home/GetMyRoutes", null, function (data) {
                allRoutes = JSON.parse(data);
                console.log(allRoutes);
                setRoutes();
            });
        } else if (chooseRoutes == -2) {
            clearMap();
            myMap.geoObjects.add(myGeolocation);
            // myMap.geoObjects.add(myGeolocation); 
            $.post("/Home/GetClosestRoutes", { positionData: jsonGeo }, function (data) {
                console.log("post");
                console.log(data);
                allRoutes = JSON.parse(data);
                console.log(allRoutes);
                setRoutes();
            });
        }
        else {
            clearMap();
            $.post("/Home/GetRouteById/" + chooseRoutes, null, function (data) {
                allRoutes = JSON.parse(data);
                setRoutes();
            });
        }
    }

    function clearMap() {
        myMap.geoObjects.removeAll();
        //myMap.geoObjects.add(myGeolocation); 
    }
    function setRoutes() {

        myMap.geoObjects.remove(routes);

        console.log("setRoutes");
        routes = [];
        for (let j = 0; j < allRoutes.routes.length; j++) {
            let route = {
                type: 'FeatureCollection',
                features: []
            };

            for (let i = 0; i < allRoutes.routes[j].marks.length; i++) {
                route.features.push({
                    type: 'Feature',
                    geometry: {
                        type:
                        'Point',
                        coordinates: [
                        allRoutes.routes[j].marks[i].x, allRoutes.routes[j].marks[i].y
                        ]
                    },
                    properties: {
                        balloonContent: allRoutes.routes[j].marks[i].name,
                        hintContent: allRoutes.routes[j].marks[i].description
                    },
                    options: {
                        preset: 'islands#violetDotIcon'
                    }
                });
            }
            console.log(route);

            routes.push(route);
        }
        showAllRoutes(routes);

    }

    function showAllRoutes(routes) {
        ymaps.ready().done(function (ym) {
            console.log(routes);
            for (let j = 0; j < routes.length; j++) {
                myPlacemarks = routes[j].features;
                console.log("placemarks: ");
                console.log(myPlacemarks);

                let multiRoute;

                if (myPlacemarks.length > 1) {
                    if (multiRoute) {
                        myMap.geoObjects.remove(multiRoute);
                    }
                    var placemarks = [];
                    for (let i = 0; i < myPlacemarks.length; i++) {
                        console.log(myPlacemarks[i].geometry.coordinates);
                        placemarks.push(myPlacemarks[i].geometry.coordinates);
                    }
                    multiRoute = new ymaps.multiRouter.MultiRoute({
                        referencePoints: placemarks,
                        params: {
                            routingMode: 'pedestrian'
                        }
                    }, {
                        balloonLayout: balloonLayout,
                        wayPointStartIconLayout: "default#image",
                        wayPointStartIconImageSize: [0, 0],
                        wayPointStartIconImageOffset: [-15, -15],
                        wayPointFinishIconLayout: "default#image",
                        wayPointFinishIconImageSize: [0, 0],
                        wayPointFinishIconImageOffset: [-15, -15],
                        routeWalkMarker: null,

                        wayPointIconLayout: "default#image",
                        wayPointIconImageSize: [0, 0],
                        wayPointIconImageOffset: [-15, -15],

                        routeActiveStrokeWidth: 6,
                        routeActivePedestrianSegmentStrokeWidth: 6,
                        editorDrawOver: false,
                        wayPointDraggable: false,
                        viaPointDraggable: false,
                        routeActivePedestrianSegmentStrokeStyle: "solid",

                        routeActivePedestrianSegmentStrokeColor: arrColors[Math.floor(Math.random() * arrColors.length)],
                        pinIconFillColor: "ff0000",
                        balloonContentBodyLayout: ymaps.templateLayoutFactory.createClass('$[properties.humanJamsTime]'),
                        boundsAutoApply: false,
                        zoomMargin: 30
                    });

                    myMap.geoObjects.remove(myPlacemarks);
                    myMap.geoObjects.add(multiRoute);
                }
            }
        }
        );
    }
});