/* global Chart:false */

//$(function () {
//  'use strict'

//  var ticksStyle = {
//    fontColor: '#495057',
//    fontStyle: 'bold'
//  }
//    var mode = 'index';
//    var intersect = true;
//    debugger;
//    let labels = $('#hdnEmpPin').val(); //@ViewBag.EmpPin;
//    let datas1 = $('#hdnPlantCount').val(); //'@ViewBag.PlantCount';
//    let datas2 = $('#hdnCallCount').val(); //'@ViewBag.CallCount';

//    //console.log(labels); 

//    var labelsArray = new Array();
//    var dataArray1 = new Array();
//    var dataArray2 = new Array();
   
//    for (var i = 0; i < labels.split(',').length; i++) {
//        labelsArray.push(labels.split(',')[i]);
//        dataArray1.push(datas1.split(',')[i]);
//        dataArray2.push(datas2.split(',')[i]);
//    }

//    //new Chart(document.getElementById("chart").getContext('2d'), {
//    //    type: 'pie',
//    //    data: {
//    //        labels: labelsArray,
//    //        datasets: [{
//    //            backgroundColor: colorArray,
//    //            data: dataArray
//    //        }]
//    //    }
//    //});

//  var $salesChart = $('#sales-chart')
//  // eslint-disable-next-line no-unused-vars
//  var salesChart = new Chart($salesChart, {
//    type: 'bar',
//    data: {
//        labels: labelsArray, //['JUN', 'JUL', 'AUG', 'SEP', 'OCT', 'NOV', 'DEC'],
//      datasets: [
//        {
//          label: 'This year',
//          backgroundColor: '#007bff',
//          borderColor: '#007bff',
//              data: dataArray1//[1000, 2000, 3000, 2500, 2700, 2500, 3000]
//        },
//          {
//           label: 'Last year',
//          backgroundColor: '#ced4da',
//          borderColor: '#ced4da',
//              data: dataArray2//[700, 1700, 2700, 2000, 1800, 1500, 2000]
//        }
//      ]
//    },
//    options: {
//      maintainAspectRatio: false,
//      tooltips: {
//        mode: mode,
//        intersect: intersect
//      },
//      hover: {
//        mode: mode,
//        intersect: intersect
//      },
//      legend: {
//        display: false
//      },
//      scales: {
//        yAxes: [{
//          // display: false,
//          gridLines: {
//            display: true,
//            lineWidth: '4px',
//            color: 'rgba(0, 0, 0, .2)',
//            zeroLineColor: 'transparent'
//          },
//          ticks: $.extend({
//            beginAtZero: true,

//            // Include a dollar sign in the ticks
//            callback: function (value) {
//              if (value >= 1000) {
//                value /= 1000
//                value += 'k'
//              }

//              return '$' + value
//            }
//          }, ticksStyle)
//        }],
//        xAxes: [{
//          display: true,
//          gridLines: {
//            display: false
//          },
//          ticks: ticksStyle
//        }]
//      }
//    }
//  })

 
//})


$(function () {

      'use strict'

      var ticksStyle = {
        fontColor: '#495057',
        fontStyle: 'bold'
      }
    var mode = 'index';
    var intersect = true;
 
    $.ajax({
        url: "/CallReport/GetPlanVsCallData",
        //data: "{ 'branchId': '" + branchId + "'}",
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            var emppinArray = [];
            var planArray = [];
            var completedArray = [];
            //console.log(data);
            $.each(data.data, function (i,item) {
                //console.log(item)
                emppinArray.push(item.empPin);
                planArray.push(item.planCount);
                completedArray.push(item.callCount);
            });
                    

            var $salesChart = $('#sales-chart')
            // eslint-disable-next-line no-unused-vars
            var salesChart = new Chart($salesChart, {
                type: 'bar',
                data: {
                    labels: emppinArray, //['JUN', 'JUL', 'AUG', 'SEP', 'OCT', 'NOV', 'DEC'],
                    datasets: [
                        {
                            label: 'Planed',
                            backgroundColor: '#007bff',
                            borderColor: '#007bff',
                            data: planArray//[1000, 2000, 3000, 2500, 2700, 2500, 3000]
                        },
                        {
                            label: 'Completed',
                            backgroundColor: '#ced4da',
                            borderColor: '#ced4da',
                            data: completedArray//[700, 1700, 2700, 2000, 1800, 1500, 2000]
                        }
                    ]
                },
                options: {
                    maintainAspectRatio: false,
                    tooltips: {
                        mode: mode,
                        intersect: intersect
                    },
                    hover: {
                        mode: mode,
                        intersect: intersect
                    },
                    legend: {
                        display: false
                    },
                    scales: {
                        yAxes: [{
                            // display: false,
                            gridLines: {
                                display: true,
                                lineWidth: '4px',
                                color: 'rgba(0, 0, 0, .2)',
                                zeroLineColor: 'transparent'
                            },
                            ticks: $.extend({
                                beginAtZero: true,

                                // Include a dollar sign in the ticks
                                callback: function (value) {
                                    if (value >= 1000) {
                                        value /= 1000
                                        value += 'k'
                                    }

                                    return '$' + value
                                }
                            }, ticksStyle)
                        }],
                        xAxes: [{
                            display: true,
                            gridLines: {
                                display: false
                            },
                            ticks: ticksStyle
                        }]
                    }
                }
            })

        },
        error: function (response) {
            alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
});

// lgtm [js/unused-local-variable]
