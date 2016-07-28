var app = angular.module('searchApp', []);

app.controller('searchCtrl', function ($scope, $http) {
    $scope.goSearch = function () {
        var myUrl = "https://scrapeme.search.windows.net/indexes/housing/docs?search=" + $scope.query + "*&api-version=2015-02-28";
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