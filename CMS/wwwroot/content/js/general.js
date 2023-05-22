if (window.addEventListener) {
    window.addEventListener('load', general_Load, false);
}
else {
    window.attachEvent('onload', general_Load);
}


function CusContains(selector, text) {
    var elements = document.querySelectorAll(selector);
    return $(Array.prototype.filter.call(elements, function (element) {
        return RegExp(text).test(element.textContent);
    }));
}


//var data = {
//    "set1": [{
//        "category": LangYes,
//        "value": SubjectModel.Yes
//    }, {
//        "category": LangNo,
//        "value": SubjectModel.No
//    }, {
//        "category": LangNotAvailable,
//        "value": SubjectModel.NotAvailable
//    }]

//}

var chartApex;
function getchartPie(sm) {



    const apexChart = "#chartdiv";
    var options = {
        series: [sm.Yes, sm.No, sm.NotAvailable, (sm.TotalSubject - (sm.Yes + sm.No + sm.NotAvailable))],
        chart: {
            width: 480,
            type: 'donut',
        },
        labels: [LangYes, LangNo, LangNotAvailable, LangCevaplanmamis],
        responsive: [{
            breakpoint: 480,
            options: {
                chart: {
                    width: 200
                },
                legend: {
                    position: 'bottom'
                }
            }
        }],
        colors: ['#a4be48', '#ee4136', '#fbb140', '#8d8d8d']
    };

    chartApex = new ApexCharts(document.querySelector(apexChart), options);
    chartApex.render();

}

var rootTemp = "";
function getchartPie4(sm) {

    am5.ready(function () {


        if (rootTemp == "")
            root = am5.Root.new("chartdiv");
        else
            root = rootTemp

        if (rootTemp == "")
            root.setThemes([
                am5themes_Animated.new(root)
            ]);


        // Create chart
        // https://www.amcharts.com/docs/v5/charts/xy-chart/
        var chart = root.container.children.push(am5xy.XYChart.new(root, {
            panX: false,
            panY: false,
            wheelX: "none",
            wheelY: "none"
        }));

        // We don't want zoom-out button to appear while animating, so we hide it

        chart.zoomOutButton.set("forceHidden", true);


        // Create axes
        // https://www.amcharts.com/docs/v5/charts/xy-chart/axes/
        var yRenderer = am5xy.AxisRendererY.new(root, {
            minGridDistance: 30
        });

        var yAxis = chart.yAxes.push(am5xy.CategoryAxis.new(root, {
            maxDeviation: 0,
            categoryField: "network",
            renderer: yRenderer,
            tooltip: am5.Tooltip.new(root, { themeTags: ["axis"] })
        }));

        var xAxis = chart.xAxes.push(am5xy.ValueAxis.new(root, {
            maxDeviation: 0,
            min: 0,
            extraMax: 0.1,
            renderer: am5xy.AxisRendererX.new(root, {})
        }));


        var series = chart.series.push(am5xy.ColumnSeries.new(root, {
            name: "Series 1",
            xAxis: xAxis,
            yAxis: yAxis,
            valueXField: "value",
            categoryYField: "network",
            tooltip: am5.Tooltip.new(root, {
                pointerOrientation: "left",
                labelText: "{valueX}"
            })
        }));


        // Rounded corners for columns
        series.columns.template.setAll({
            cornerRadiusTR: 5,
            cornerRadiusBR: 5
        });



        // Make each column to be of a different color
        series.columns.template.adapters.add("fill", function (fill, target) {
            return chart.get("colors").getIndex(series.columns.indexOf(target));
        });

        series.columns.template.adapters.add("stroke", function (stroke, target) {
            return chart.get("colors").getIndex(series.columns.indexOf(target));
        });




        // Set data
        var data = [
            {
                "network": LangYes,
                "value": sm.Yes,
            },
            {
                "network": LangNo,
                "value": sm.No
            },
            {
                "network": LangNotAvailable,
                "value": sm.NotAvailable
            }
        ];


        yAxis.data.setAll(data);

        series.data.setAll(data);

        //if (rootTemp!="")
        //    

        //    am5.array.each(series.dataItems, function (dataItem) {
        //        // both valueY and workingValueY should be changed, we only animate workingValueY
        //        dataItem.set("valueX", value);
        //        //dataItem.animate({
        //        //    key: "valueXWorking",
        //        //    to: value,
        //        //    duration: 600,
        //        //    easing: am5.ease.out(am5.ease.cubic)
        //        //});
        //    })

        //    sortCategoryAxis();

        //    return;
        //}


        sortCategoryAxis();

        // Get series item by category
        function getSeriesItem(category) {
            for (var i = 0; i < series.dataItems.length; i++) {
                var dataItem = series.dataItems[i];
                if (dataItem.get("categoryY") == category) {
                    return dataItem;
                }
            }
        }
        chart.set("cursor", am5xy.XYCursor.new(root, {
            behavior: "none",
            xAxis: xAxis,
            yAxis: yAxis
        }));


        // Axis sorting
        function sortCategoryAxis() {

            // Sort by value
            series.dataItems.sort(function (x, y) {
                return x.get("valueX") - y.get("valueX"); // descending
                //return y.get("valueY") - x.get("valueX"); // ascending
            })

            // Go through each axis item
            am5.array.each(yAxis.dataItems, function (dataItem) {
                // get corresponding series item
                var seriesDataItem = getSeriesItem(dataItem.get("category"));

                if (seriesDataItem) {
                    // get index of series data item
                    var index = series.dataItems.indexOf(seriesDataItem);
                    // calculate delta position
                    var deltaPosition = (index - dataItem.get("index", 0)) / series.dataItems.length;
                    // set index to be the same as series data item index
                    dataItem.set("index", index);
                    // set deltaPosition instanlty
                    dataItem.set("deltaPosition", -deltaPosition);
                    // animate delta position to 0
                    dataItem.animate({
                        key: "deltaPosition",
                        to: 0,
                        duration: 1000,
                        easing: am5.ease.out(am5.ease.cubic)
                    })
                }
            });

            // Sort axis items by index.
            // This changes the order instantly, but as deltaPosition is set,
            // they keep in the same places and then animate to true positions.
            yAxis.dataItems.sort(function (x, y) {
                return x.get("index") - y.get("index");
            });
        }






        // Make stuff animate on load
        // https://www.amcharts.com/docs/v5/concepts/animations/
        series.appear(1000);
        chart.appear(1000, 100);
        rootTemp = root;
    }); // end am5.ready()

}

function getchartPie3(SubjectModel) {


    var data = {
        "set1": [{
            "category": LangYes,
            "value": SubjectModel.Yes
        }, {
            "category": LangNo,
            "value": SubjectModel.No
        }, {
            "category": LangNotAvailable,
            "value": SubjectModel.NotAvailable
        }]

    }

    am4core.useTheme(am4themes_animated);

    // Create chart instance
    var chart = am4core.create("chartdiv", am4charts.XYChart);

    // Add data
    chart.data = data.set1;

    // Create axes
    var categoryAxis = chart.xAxes.push(new am4charts.CategoryAxis());
    categoryAxis.dataFields.category = "category";
    categoryAxis.renderer.grid.template.location = 0;
    //categoryAxis.renderer.minGridDistance = 30;

    var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());

    // Create series
    var series = chart.series.push(new am4charts.ColumnSeries());
    series.dataFields.valueY = "value";
    series.dataFields.categoryX = "category";

    //function selectDataset(set) {
    //    chart.data = data[set];
    //}
}

function getchartPie2(SubjectModel) {


    am5.ready(function () {

        // Create root element
        // https://www.amcharts.com/docs/v5/getting-started/#Root_element
        var root = am5.Root.new("chartdiv");

        // Set themes
        // https://www.amcharts.com/docs/v5/concepts/themes/
        root.setThemes([
            am5themes_Animated.new(root)
        ]);

        var data = [{
            name: "Yes",
            steps: SubjectModel.Yes,
            pictureSettings: {
                src: "/images/yes.png"
            }
        }, {
            name: "No",
            steps: SubjectModel.No,
            pictureSettings: {
                src: "/images/no.png"
            }
        }, {
            name: "Not Available",
            steps: SubjectModel.NotAvailable,
            pictureSettings: {
                src: "/images/not.png"
            }
        }
        ];

        // Create chart
        // https://www.amcharts.com/docs/v5/charts/xy-chart/
        var chart = root.container.children.push(
            am5xy.XYChart.new(root, {
                panX: false,
                panY: false,
                wheelX: "none",
                wheelY: "none",
                paddingBottom: 50,
                paddingTop: 40
            })
        );

        // Create axes
        // https://www.amcharts.com/docs/v5/charts/xy-chart/axes/

        var xRenderer = am5xy.AxisRendererX.new(root, {});
        xRenderer.grid.template.set("visible", false);

        var xAxis = chart.xAxes.push(
            am5xy.CategoryAxis.new(root, {
                paddingTop: 40,
                categoryField: "name",
                renderer: xRenderer
            })
        );


        var yRenderer = am5xy.AxisRendererY.new(root, {});
        yRenderer.grid.template.set("strokeDasharray", [3]);

        var yAxis = chart.yAxes.push(
            am5xy.ValueAxis.new(root, {
                min: 0,
                renderer: yRenderer
            })
        );

        // Add series
        // https://www.amcharts.com/docs/v5/charts/xy-chart/series/
        var series = chart.series.push(
            am5xy.ColumnSeries.new(root, {
                name: "Income",
                xAxis: xAxis,
                yAxis: yAxis,
                valueYField: "steps",
                categoryXField: "name",
                sequencedInterpolation: true,
                calculateAggregates: true,
                maskBullets: false,
                tooltip: am5.Tooltip.new(root, {
                    dy: -30,
                    pointerOrientation: "vertical",
                    labelText: "{valueY}"
                })
            })
        );

        series.columns.template.setAll({
            strokeOpacity: 0,
            cornerRadiusBR: 10,
            cornerRadiusTR: 10,
            cornerRadiusBL: 10,
            cornerRadiusTL: 10,
            maxWidth: SubjectModel.TotalSubject,
            fillOpacity: 0.8
        });

        var currentlyHovered;

        series.columns.template.events.on("pointerover", function (e) {
            handleHover(e.target.dataItem);
        });

        series.columns.template.events.on("pointerout", function (e) {
            handleOut();
        });

        function handleHover(dataItem) {
            if (dataItem && currentlyHovered != dataItem) {
                handleOut();
                currentlyHovered = dataItem;
                var bullet = dataItem.bullets[0];
                bullet.animate({
                    key: "locationY",
                    to: 1,
                    duration: 750,
                    easing: am5.ease.out(am5.ease.cubic)
                });
            }
        }

        function handleOut() {
            if (currentlyHovered) {
                var bullet = currentlyHovered.bullets[0];
                bullet.animate({
                    key: "locationY",
                    to: 0,
                    duration: 750,
                    easing: am5.ease.out(am5.ease.cubic)
                });
            }
        }

        var circleTemplate = am5.Template.new({});

        series.bullets.push(function (root, series, dataItem) {
            var bulletContainer = am5.Container.new(root, {});
            var circle = bulletContainer.children.push(
                am5.Circle.new(
                    root,
                    {
                        radius: 34
                    },
                    circleTemplate
                )
            );

            var maskCircle = bulletContainer.children.push(
                am5.Circle.new(root, { radius: 27 })
            );

            // only containers can be masked, so we add image to another container
            var imageContainer = bulletContainer.children.push(
                am5.Container.new(root, {
                    mask: maskCircle
                })
            );

            var image = imageContainer.children.push(
                am5.Picture.new(root, {
                    templateField: "pictureSettings",
                    centerX: am5.p50,
                    centerY: am5.p50,
                    width: 50,
                    height: 50
                })
            );

            return am5.Bullet.new(root, {
                locationY: 0,
                sprite: bulletContainer
            });
        });

        // heatrule
        series.set("heatRules", [
            {
                dataField: "valueY",
                min: am5.color(0xe5dc36),
                max: am5.color(0x5faa46),
                target: series.columns.template,
                key: "fill"
            },
            {
                dataField: "valueY",
                min: am5.color(0xe5dc36),
                max: am5.color(0x5faa46),
                target: circleTemplate,
                key: "fill"
            }
        ]);

        series.data.setAll(data);
        xAxis.data.setAll(data);

        var cursor = chart.set("cursor", am5xy.XYCursor.new(root, {}));
        cursor.lineX.set("visible", false);
        cursor.lineY.set("visible", false);

        cursor.events.on("cursormoved", function () {
            var dataItem = series.get("tooltip").dataItem;
            if (dataItem) {
                handleHover(dataItem);
            } else {
                handleOut();
            }
        });

        // Make stuff animate on load
        // https://www.amcharts.com/docs/v5/concepts/animations/
        series.appear();
        chart.appear(1000, 100);

    }); // end am5.ready()

}




function setCharts(list, id) {

    var labels = [];
    var data = [];
    var backgroundColor = [];
    $.each(list, function (i, row) {
        labels.push(row.Name);
        data.push(row.count);
        backgroundColor.push(row.color);
    });


    var e = {
        type: "doughnut",
        data: {
            datasets: [{
                data: data,
                backgroundColor: backgroundColor,
            }],
            labels: labels
        },
        options: {
            cutoutPercentage: 75,
            responsive: !0,
            maintainAspectRatio: !1,
            legend: {
                display: !1,
                position: "top"
            },
            title: {
                display: !1,
                text: "Technology"
            },
            animation: {
                animateScale: !0,
                animateRotate: !0
            },
            tooltips: {
                enabled: !0,
                intersect: !1,
                mode: "nearest",
                bodySpacing: 5,
                yPadding: 10,
                xPadding: 10,
                caretPadding: 0,
                displayColors: !1,
                //backgroundColor: KTApp.getStateColor("brand"),
                titleFontColor: "#ffffff",
                cornerRadius: 4,
                footerSpacing: 0,
                titleSpacing: 0
            }
        }
    },
        a = document.querySelector('#kt_chart_profit_share_' + id).getContext('2d');
    //a = KTUtil.getByID("kt_chart_profit_share_" + id).getContext("2d");
    var ch = new AmCharts(a, e)

    return ch;

}


function setCatColorBack(divid) {
    var divtext = $(divid);
    var value = divtext.text();
    if (divtext.text() == "") {
        value = divtext.val();
    }

    if (value == "") {
        value = divtext.attr("data");
    }

    if (value != undefined) {

        if (value.indexOf('Yes') != -1) {
            divtext.attr('style', divtext.attr('style') + '; ' + 'background-color:rgb(149, 193, 31) !important');
            //divtext.css('background-color', 'rgb(149, 193, 31)');
        }
        else if (value.indexOf('Not') != -1) {
            divtext.attr('style', divtext.attr('style') + '; ' + 'background-color:rgb(255, 92, 53) !important');
            //divtext.css('background-color', ' rgb(255, 92, 53)');
        }
        else if (value.indexOf('No') != -1) {
            divtext.attr('style', divtext.attr('style') + '; ' + 'background-color:rgb(45, 204, 205) !important');
            //divtext.css('background-color', 'rgb(45, 204, 205)');
        }
        else {
            divtext.attr('style', divtext.attr('style') + '; ' + 'background-color:#65646b');
            //divtext.css('background-color', '#65646b');
        }
    }
    else {
        divtext.attr('style', divtext.attr('style') + '; ' + 'background-color:#65646b !important');
        //divtext.css('background-color', '#65646b ');
    }
}


function toChartRow(list, id, Name, Desc, dataCount) {
    $(id).html("");
    var str = "";


    str += '   <div class="kt-portlet kt-portlet--height-fluid">                                                           ';
    str += '       <div class="kt-widget14">                                                                               ';
    str += '           <div class="kt-widget14__header">                                                                   ';
    str += '               <h3 class="kt-widget14__title">  ' + Name + '</h3>';
    str += '                 <span class="kt-widget14__desc">' + Desc + '</span>                                                                                      ';
    str += '           </div>                                                                                              ';
    str += '           <div class="kt-widget14__content">                                                                  ';
    str += '               <div class="kt-widget14__chart">                                                                ';
    str += '                   <div class="kt-widget14__stat"></div>                                     ';
    str += '                   <canvas id="kt_chart_profit_share_' + $(id)[0].id + '" style="height: 200px; width: 200px;"></canvas>           ';
    //str += '                   <div id="kt_chart_profit_share_' + $(id)[0].id + '" style="height: 250px; width: 250px;"></div>           ';
    str += '               </div>                                                                                          ';
    str += '               <div class="kt-widget14__legends">                                                              ';
    $.each(list, function (i, row) {
        str += '                   <div class="kt-widget14__legend">                                                           ';
        str += '                       <span data="' + row.Name + '" class="catColorCustomBack kt-widget14__bullet kt-bg-success"></span>                                 ';
        str += '                       <span class="kt-widget14__stats ">' + row.Name + ' ' + row.count + " Adet" + '</span>              ';
        str += '                   </div>                                                                                      ';
    });
    str += '               </div>                                                                                          ';
    str += '           </div>                                                                                              ';
    str += '       </div>                                                                                                  ';
    str += '   </div>                                                                                                      ';




    $(id).append(str);

    $.each($('.catColorCustomBack'), function (i, item) {
        setCatColorBack(item);
    });


    //toChart(list, $(id)[0].id);
    //toChartNew(list, $(id)[0].id);
    setCharts(list, $(id)[0].id);

    $(id).LoadingOverlay("hide");


}


function toChartNew(list, id) {

    var series = [];
    for (var i = 0; i < list.length; i++) {
        var row = list[i];

        var color = '#65646b';
        switch (i) {
            case 1: {
                color = '#95c11f';
                break;
            }
            case 2: {
                color = '#ff5c35';
                break;
            }
            case 3: {
                color = '#2dcccd';
                break;
            }
            case 4: {
                color = '#65646b';
                break;
            }
            case 5: {
                color = '#65646b';
                break;
            }
            default: {
                color = '#65646b';
                break;
            }
        }
        //mApp.getColor(color)
        //var newRow = {
        //    value: row.count, className: "custom", meta: { color: color }
        //}
        var newRow = {
            //'name': row.Name,
            'value': row.count,
            //'fill': am4core.color(color),
            //'color': am4core.color(color),
            'color': color,


        };
        series.push(newRow);
    }

    var chart = AmCharts.makeChart('kt_chart_profit_share_4' + id, {
        "type": "pie",
        "theme": "light",

        "fontFamily": 'Open Sans',

        "color": '#888',

        "dataProvider": series,
        "valueField": "value",
        //'colorField': 'color',
        'fill': 'color'
        //"titleField": "name",
        //"exportConfig": {
        //    menuItems: [{
        //        icon: App.getGlobalPluginsPath() + "amcharts/amcharts/images/export.png",
        //        format: 'png'
        //    }]
        //}
    });

    //chart.dataFields.hidden = "hidden";

    $('#kt_chart_profit_share_4' + id).closest('.portlet').find('.fullscreen').click(function () {
        chart.invalidateSize();
    });


}



function isFloat(n) {
    return Number(n) === n && n % 1 !== 0;
}

function getQuery(name, url = window.location.href) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

function redirect(url) {
    var ua = navigator.userAgent.toLowerCase(),
        isIE = ua.indexOf('msie') !== -1,
        version = parseInt(ua.substr(4, 2), 10);

    // Internet Explorer 8 and lower
    //if (isIE && version < 9) {
    var link = document.createElement('a');
    link.href = url;
    document.body.appendChild(link);
    link.click();
    //}

    // All other browsers can use the standard window.location.href (they don't lose HTTP_REFERER like Internet Explorer 8 & lower does)
    //else {
    //    window.location.href = url;
    //}
}



function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('#') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}





function general_Load() {
    window.onbeforeunload = setClick;
}



var postArray = [];
function setClick() {
    for (var i = 0; i < postArray.length; i++) {
        postArray[i].abort();
    }
    // cl();
}


var toStr = function (e) {
    return e == undefined || e == "undefined" || e == null || e == "null" ? "" : e.toString();
};

var toInt = function (e) {
    return e == undefined || e == "undefined" || e == null || e == "null" ? 0 : parseInt(e);
};

var toFloat = function (e) {
    return e == "" || e == undefined || e == "undefined" || e == null || e == "null" ? 0 : parseFloat(e);
};

var toFloat1 = function (e) {
    return e == "" || e == undefined || e == "undefined" || e == null || e == "null" ? 1 : parseFloat(e);
};


var isBool = function (variable) {
    if (typeof (variable) === "boolean") {
        return true;
    }
    else {
        return false;
    }
}

var toBool = function (value) {
    return value == true ? "True" : "False";
}

var isNumeric = function (n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

jQuery('.isnumeric').keyup(function () {
    this.value = this.value.replace(/[^0-9\.]/g, '');
});




function GetSpecListTypeCustom(SpecListType, name, selectValue, selectText) {
    $('#dp_' + name + '').html('');
    $.get('/Spec/GetSpecListTypeList', { SpecListType: SpecListType, name: selectText }).fail(function (err, exception) { console.error(err.responseText); })
        .done(function (res) {
            $('#dp_' + name + '').append($('<option>', { value: "", text: selectText + ' ' + LangSelect }));

            $.each(res.ResultRow?.SpecAttrs, function (i, item) {
                var LangName = getLangSpecAttr(item);
                var str = '<option  value="' + item.Id + '" ' + (res.name == item.Id ? 'selected="selected"' : '') + '>' + LangName + (item.Desc != null ? ' (' + item.Desc + ')' : '') + '</option>';
                $('#dp_' + name + '').append(str);
            });

            $('#dp_' + name + '').select2();

        });
}


function GetSpecListType(SpecListType, name) {
    $('#dp_' + name + '').html('');
    $.get('/Spec/GetSpecListTypeList', { SpecListType: SpecListType, name: name }).fail(function (err, exception) { console.error(err.responseText); })
        .done(function (res) {
            $('#dp_' + res.Joker + '').append($('<option>', { value: "", text: LangSelect }));
            var joker = rowProject[res.Joker];
            $.each(res.ResultRow?.SpecAttrs, function (i, item) {
                var LangName = getLangSpecAttr(item);
                var str = '<option  value="' + item.Id + '" ' + (joker == item.Id ? 'selected="selected"' : '') + '>' + LangName + (item.Desc != null ? ' (' + item.Desc + ')' : '') + '</option>';
                $('#dp_' + res.Joker + '').append(str);
            });

        });
}


function GetSpecListTypeHazard(SpecListType, name) {
    $('#dp_' + name + '').html('');
    $.get('/Spec/GetSpecListTypeList', { SpecListType: SpecListType, name: name }).fail(function (err, exception) { console.error(err.responseText); })
        .done(function (res) {
            $('#dp_' + res.Joker + '').append($('<option>', { value: "", text: LangSelect }));
            var joker = rowHazard[res.Joker];
            $.each(res.ResultRow?.SpecAttrs, function (i, item) {
                var LangName = getLangSpecAttr(item);
                var str = '<option  value="' + item.Id + '" ' + (joker == item.Id ? 'selected="selected"' : '') + '>' + LangName + (item.Desc != null ? ' (' + item.Desc + ')' : '') + '</option>';
                $('#dp_' + res.Joker + '').append(str);
            });

        });
}

function GetSpecListTypeParent(SpecListType, name, ParentId) {
    $('#dp_' + name + '').html('');
    $.get('/Spec/GetSpecListTypeListParent', { SpecListType: SpecListType, name: name, ParentId: ParentId }).fail(function (err, exception) { console.error(err.responseText); })
        .done(function (res) {
            $('#dp_' + res.Joker + '').append($('<option>', { value: "", text: LangSelect }));
            var joker = rowHazard[res.Joker];
            $.each(res.ResultRow?.SpecAttrs, function (i, item) {
                var LangName = getLangSpecAttr(item);
                var str = '<option  value="' + item.Id + '" ' + (joker == item.Id ? 'selected="selected"' : '') + '>' + LangName + (item.Desc != null ? ' (' + item.Desc + ')' : '') + '</option>';
                $('#dp_' + res.Joker + '').append(str);
            });

            if (toInt($('#dp_' + name + '').val()) > 0 && ParentId < 1) {
                GetSpecListTypeParent(SpecListType, 'HazardTypeSubId', $('#dp_' + name + '').val());
            }

        });

}



function getLangSpecAttr(item) {
    var text = '';
    switch (LanguageId) {
        case 1:
            {
                text = item.Name;
            }
            break;
        case 2:
            {
                text = item.NameLang1;
            }
            break;
        case 3:
            {
                text = item.NameLang2;
            }
            break;
        case 4:
            {
                text = item.NameLang3;
            }
            break;
        case 5:
            {
                text = lang.NameLang4;
            }
            break;
    }
    return text;
}



function getLangHazardRating(item) {
    var text = '';
    switch (LanguageId) {
        case 1:
            {
                text = item.RatingMeasureName;
            }
            break;
        case 2:
            {
                text = item.RatingMeasureName1;
            }
            break;
        case 3:
            {
                text = item.RatingMeasureName2;
            }
            break;
        case 4:
            {
                text = item.RatingMeasureName3;
            }
            break;
        case 5:
            {
                text = lang.RatingMeasureName4;
            }
            break;
    }
    return text;
}




function getLang() {
    $.get('/Lang/GetSelect').fail(function (err, exception) { console.error(err.responseText); })
        .done(function (res) {
            $('#dp_LangId').append($('<option>', { value: "", text: LangSelect }));
            var LangId = "";
            try {
                LangId = rowProject.LangId;
            } catch (e) {

            }
            $.each(res, function (i, item) {
                var str = '<option  value="' + item.value + '" ' + (LangId == item.value ? 'selected="selected"' : '') + '>' + item.text + '</option>';
                $('#dp_LangId').append(str);
            });

        });
}



function GetContentPage(ContentTypes, Joker, Joker2) {
    $('#' + Joker).html('');
    $.get('/ContentPage/GetSelectCustom?ContentTypes=' + ContentTypes + '&Joker=' + Joker + '&Joker2=' + Joker2).fail(function (err, exception) { console.error(err.responseText); })
        .done(function (res) {
            var Joker = res.Joker;
            var Joker2 = res.Joker2;
            $('#' + Joker).append($('<option>', { value: "", text: LangSelect }));
            $.each(res.ResultList, function (i, item) {
                var str = '<option  value="' + item.value + '" ' + '>' + item[Joker2] + '</option>';
                $('#' + Joker).append(str);
            });

            $('#' + Joker).select2({});
        });
}


function GetTypeAll(AssetsId, HazardTypeId, HazardTypeSubId, Joker, Joker2) {
    $('#' + Joker).html('');
    $.get('/AssetsStandartList/GetTypeAll?AssetsId=' + AssetsId + '&HazardTypeId=' + HazardTypeId + '&HazardTypeSubId=' + HazardTypeSubId + '&Joker=' + Joker + '&Joker2=' + Joker2).fail(function (err, exception) { console.error(err.responseText); })
        .done(function (res) {
            var Joker = res.Joker;
            var Joker2 = res.Joker2;
            $('#' + Joker).append($('<option>', { value: "", text: LangSelect }));
            $.each(res.ResultList, function (i, item) {
                var str = '<option  value="' + item.Id + '" ' + '>' + item[Joker2] + '</option>';
                $('#' + Joker).append(str);
            });

            $('#' + Joker).select2({});
        });
}

function GetTypeAllStandartDetail(AssetsStandartListId, HazardTypeId, HazardTypeSubId, Joker, Joker2) {
    $('#' + Joker).html('');
    $.get('/StandartDetail/GetTypeAll?AssetsStandartListId=' + AssetsStandartListId + '&HazardTypeId=' + HazardTypeId + '&HazardTypeSubId=' + HazardTypeSubId + '&Joker=' + Joker + '&Joker2=' + Joker2).fail(function (err, exception) { console.error(err.responseText); })
        .done(function (res) {
            var Joker = res.Joker;
            var Joker2 = res.Joker2;
            $('#' + Joker).append($('<option>', { value: "", text: LangSelect }));
            $.each(res.ResultList, function (i, item) {
                var str = '<option  value="' + item.Id + '" ' + '>' + item[Joker2] + '</option>';
                $('#' + Joker).append(str);
            });

            $('#' + Joker).select2({});
        });
}


function GetCustomFilter(Joker, AssetsId, HazardTypeId, HazardTypeSubId, StandartDetailId) {
    $('#' + Joker).html('');
    $.get('/AssetsStandartList/GetCustomFilter?Joker=' + Joker + '&AssetsId=' + AssetsId + '&HazardTypeId=' + HazardTypeId + '&HazardTypeSubId=' + HazardTypeSubId + '&StandartDetailId=' + StandartDetailId).fail(function (err, exception) { console.error(err.responseText); })
        .done(function (res) {
            var Joker = res.Joker;

            $('#' + Joker).append($('<option>', { value: "", text: LangSelect }));
            $.each(res.ResultList, function (i, item) {
                var str = '<option  value="' + item.Id + '" ' + '>' + item.HazardDesc + '</option>';
                $('#' + Joker).append(str);
            });

            $('#' + Joker).select2({});
        });
}





function getObjects(obj, key, val) {
    var objects = [];
    for (var i in obj) {
        if (!obj.hasOwnProperty(i)) continue;
        if (typeof obj[i] == 'object') {
            objects = objects.concat(getObjects(obj[i], key, val));
        } else
            if (i == key && obj[i] == val || i == key && val == '') { //
                objects.push(obj);
            } else if (obj[i] == val && key == '') {
                if (objects.lastIndexOf(obj) == -1) {
                    objects.push(obj);
                }
            }
    }
    return objects;
}



function getEnumList(dataResult, id, selectid, selectText) {
    $(id).addOption(dataResult, "value", "text", null, null, selectid, '', selectText);
}

function getEnumRow(dataResult, value) {
    if (value < 1) {
        row = { value: "", text: "", name: "" };
        return row;
    }
    var row;
    for (var i = 0; i < dataResult.length; i++) {
        var rowItem = dataResult[i];
        if (parseInt(rowItem.value) == parseInt(value)) {
            row = rowItem;
            break;
        }
    }
    return row;
}

function getEnumRowName(dataResult, value) {
    if (value < 1) {
        row = { value: "", text: "", name: "" };
        return row;
    }
    var row;
    for (var i = 0; i < dataResult.length; i++) {
        var rowItem = dataResult[i];
        if (rowItem.name == value) {
            row = rowItem;
            break;
        }
    }
    return row;
}




(function ($) {
    $.fn.ceo = function (ayarlar) {
        var ayar = $.extend({
            'source': "#" + $(this)[0].name,
            'target': "",
        }, ayarlar);

        $(ayar.source).on("keyup", function () {
            str = $(this).val();
            str = replaceSpecialChars(str);
            str = str.toLowerCase();
            str = str.replace(/\s\s+/g, ' ').replace(/[^a-z0-9\s]/gi, '').replace(/[^\w]/ig, "-");
            function replaceSpecialChars(str) {
                var specialChars = [["ş", "s"], ["ğ", "g"], ["ü", "u"], ["ı", "i"], ["_", "-"],
                ["ö", "o"], ["Ş", "S"], ["Ğ", "G"], ["Ç", "C"], ["ç", "c"],
                ["Ü", "U"], ["İ", "I"], ["Ö", "O"], ["ş", "s"]];
                for (var i = 0; i < specialChars.length; i++) {
                    str = str.replace(eval("/" + specialChars[i][0] + "/ig"), specialChars[i][1]);
                }
                return str;
            }
            $(ayar.target).val(str);
        });
    };

})(jQuery);

(function ($) {
    $.fn.dup = function (ayarlar) {
        var ayar = $.extend({
            'source': "#" + $(this)[0].name,
            'target': "",
        }, ayarlar);

        $(ayar.source).on("keyup", function () {
            str = $(this).val();
            $(ayar.target).val(str);
        });
    };

})(jQuery);


function alerts(message, button, call) {
    var but = [];

    if (button == "yesno") {
        but = {
            confirm: {
                label: "Yes",
                className: 'btn-success'
            },
            cancel: {
                label: "No",
                className: 'btn-danger'
            }
        };
        //bootbox.alert(message);
        bootbox.confirm({
            message: message,
            buttons: but,
            callback: call,
        });
        return;
    }
    else if (button == "ok") {
        but = {
            confirm: {
                label: "Ok",
                className: 'btn-success'
            }
        };
        if (call != null && call != undefined) {
            bootbox.confirm({
                message: message,
                buttons: but,
                callback: call,
            });
        }
        else {
            bootbox.alert({
                message: message,
            });
        }
    }
    else {

        if (call != null && call != undefined) {
            but = button;
            bootbox.confirm({
                message: message,
                buttons: but,
                callback: call,
            });
        }
        else {
            bootbox.alert({
                message: message,
            });
        }
    }



}


(function ($) {
    "use strict";



    var postArray = [];
    $.ajx = function (url, postmodel, successMethod, error) {
        //if (typeof data != "string" && data != null) {
        //    data = JSON.stringify(data);
        //}
        var slash = url.substr(0, 1) == "/" ? "" : "/";

        var post = $.post(slash + url, postmodel, successMethod)
            .fail(function (e, exception) {
                if (error) {
                    var errorResult = $.errorSend(e, exception);
                    console.log(exception);
                }
            });
        postArray.push(post);
    };

    $.ajxUpload = function (url, data, successMethod, error) {
        //if (typeof data != "string" && data != null) {
        //    data = JSON.stringify(data);
        //}
        var slash = url.substr(0, 1) == "/" ? "" : "/";

        $.ajax({
            url: slash + url,
            type: "POST",
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  
            data: data,
            success: successMethod,
            error: function (e, exception) {
                if (error) {
                    var errorResult = $.errorSend(e, exception);
                    console.log(errorResult);
                }
            }
        });

    };

    $.errorSend = function (e, exception) {
        var error = '';
        if (e.status === 0) {
            error = 'Not connect. Verify Network.';
        } else if (e.status === 404) {
            error = 'Requested page not found. [404]';
        } else if (e.status === 500) {
            error = 'Internal Server Error [500].';
        } else if (exception === 'parsererror') {
            error = 'Requested JSON parse failed.';
        } else if (exception === 'timeout') {
            error = 'Time out error.';
        } else if (exception === 'abort') {
            error = 'Ajax request aborted.';
        } else {
            error = 'Uncaught Error. \n' + error.responseText;
        }
        var data = { error: error };

        return error;
    };

    $.fn.addOption = function (selectValues, value, text, dpChange, dpSuccess, selectValue, selectText, selectDefault, attrName) {

        var thisid = $(this);
        $.each(thisid, function (i, id) {
            $(id).html('');
            if (selectDefault) {
                var optionsAll = $("<option></option>")
                    .attr("value", '')
                    .text(selectDefault);
                $(id).append(optionsAll);
            }

            $.each(selectValues, function (i, item) {

                var splitText = text.split(',');
                var textValue = "";
                for (var ii = 0; ii < splitText.length; ii++)
                    textValue += item[splitText[ii]] + (ii >= 0 ? ' ' : '');


                var optionsAll = $("<option></option>")
                    .attr("value", item[value])
                    .text( textValue);

                if (attrName)
                    $(optionsAll)
                        .attr(attrName, item[attrName]);


                if (selectValue != null && selectValue != "" && value != '' && value != undefined && selectValue.toString() == item[value].toString()) {
                    $(optionsAll).attr('selected', "selected");
                }
                if (selectText != null && text != '' && text != undefined && selectText == item[text]) {
                    $(optionsAll).attr('selected', "selected");
                }

                var sectionid = toInt($(id).attr('sectionid'));
                if (sectionid > 0 && sectionid == item.value) {
                    $(optionsAll).attr('selected', "selected");
                }

                //var contentid = $(id).attr('contentid');
                //if (contentid > 0 && contentid == item.value) {
                //    $(optionsAll).attr('selected', "selected");
                //}


                $(id).append(optionsAll);

            });

            if (dpChange != null) {
                $(document).on('change', id, function (e) { dpChange(e.target); });
            }
            if (dpSuccess != null) {
                dpSuccess(id);
                //$(document).on('', id, function (e) { dpSuccess(e.target); });
            }
            //try {
            //    $(id).select2({});
            //} catch (e) {
            //    console.log(e);
            //}

        });



    };

    $.fn.addOptionAjax = function (url, param, value, text, dpChange, dpSuccess, selectValue, selectText, selectDefault, attrName) {
        var id = this;
        $(id).LoadingOverlay("show");
        $(id).html('');

        var slash = url.substr(0, 1) == "/" ? "" : "/";

        $.ajx(slash + url, param, function (dataResult) {
            try {
                var datas = dataResult.ResultList;
                if (dataResult.Joker != null && dataResult.Joker != '')
                    selectValue = dataResult.Joker2;

                if (datas != undefined)
                    dataResult = datas;
            } catch (e) {

            }
            $(id).addOption(dataResult, value, text, dpChange, dpSuccess, selectValue, selectText, selectDefault, attrName);
            $(id).LoadingOverlay("hide");
        }, null);

    };


    $.fn.toForm = function (id) {//serialize data function

        var formArray = $(id).serializeArray();
        var returnArray = {};

        for (var i = 0; i < formArray.length; i++) {
            var finame = (formArray[i]['name']).toString();
            var fivalue = (formArray[i]['value']).toString();
            if (finame.indexOf("Id") != -1 && fivalue == "-1") {
                returnArray[formArray[i]['name']] = null;
            }
            else {
                if (finame) {
                    if (finame.indexOf('file_link_') != -1) {
                        returnArray[finame.replace('file_link_', '')] = fivalue.trim();
                    }
                    else
                        returnArray[formArray[i]['name']] = fivalue.trim();
                }

            }
        }
        $(id + ' select[disabled],' + id + ' select').each(function () {
            var dp = $(this).attr('name');
            try {
                if (dp.indexOf('dp_') != -1) {
                    dp = dp.substr(3, dp.length - 1);
                }
            } catch (e) {
                //console.log('select error: ' + e);
            }

            returnArray[dp] = $(this).val();
            if (isFloat(returnArray[dp]))
                returnArray[dp] = parseFloat(returnArray[dp]);

            //try {
            //    if (dp.indexOf('Id') != -1 && ($(this).val() == "-1" || $(this).val() == "0" || $(this).val() == "")) {
            //        returnArray[dp] = null;
            //    }
            //} catch (e) {
            //    console.log(e);
            //}


            delete returnArray[$(this).attr('name')];

        });
        $(id + ' input[disabled]').each(function (i, item) {
            if ($(this).attr('name')) {
                if ($(this).attr('name').indexOf('file_link_') != -1) {
                    returnArray[$(this).attr('name').replace('file_link_', '')] = $(this).val().trim();
                }
                else
                    returnArray[$(this).attr('name')] = $(this).val().trim();
            }

        });

        $(id + ' textarea').each(function () {
            try {
                returnArray[$(this).attr('name')] = $(this).val();

            } catch (e) {
                console.error(e);
            }

        });


        $(id + ' input[type="checkbox"]').each(function () {
            returnArray[$(this).attr('name')] = $(this).prop("checked");
        });

        return returnArray;
    };

    $.getAttrs = function (attrName) {
        var data = this;
        var attrValues = [];
        $.each(data, function (i, item) {
            attrValues.push({ id: $(item).attr(attrName) });
        });
        return attrValues;
    };


})(jQuery);