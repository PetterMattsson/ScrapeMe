var app = angular.module('searchApp', []);

app.controller('searchCtrl', function ($scope, $http) {
    //Filter values
    $scope.fee = 5000;
    $scope.rooms = 1;
    $scope.size = 30;

    $scope.goSearch = function () {
        var myFilter = "";
        if ($scope.fee > 0) {
            myFilter += "&$filter=Fee le " + $scope.fee;
        }
        if ($scope.rooms > 0 && myFilter ==="") {
            myFilter += "&$filter=Rooms ge " + $scope.rooms;
        }
        else {
            myFilter += "and Rooms ge " + $scope.rooms;
        }
        if ($scope.size > 0 && myFilter==="") {
            myFilter += "&$filter=Size ge " + $scope.size;
        }
        else {
            myFilter += "and Size ge " + $scope.size;
        }
        var myUrl = "https://scrapeme.search.windows.net/indexes/housing/docs?search=" + $scope.query +"*"+ myFilter +"&api-version=2015-02-28";
        myUrl = encodeURI(myUrl);
        var req = {
            method: 'GET',
            url: myUrl,
            headers: {
                'Content-Type': 'application/JSON',
                'api-key': '702D2B776E3D459E128ACCCAECD01BD9'
            }
        };

        $http(req)
           .then(function (response) {
               $scope.records = response.data.value;
           });
    };

   
});