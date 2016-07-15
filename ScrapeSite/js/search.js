/*Binding*/

$( "#searchButton" ).click(function() {
  searchQuery($("#qV").val());
});

/*Query - https://scrapeme.search.windows.net/indexes/housing/docs?search=Nybro&api-version=2015-02-28*/
function searchQuery (query){
	var app = angular.module('searchApp', ['ngRoute']);
	
	app.controller('myCtrl', function($scope, $http) {
    $http({
        method : "GET",
        url : "https://scrapeme.search.windows.net/indexes/housing/docs?search=Nybro&api-version=2015-02-28"
    }).then(function mySucces(response) {
        $scope.myResult = response.data;
    }, function myError(response) {
        $scope.myResult = response.statusText;
    });
});
}
/*Result generation*/


/*
		$( "<img>" ).attr( "src", item.media.m ).appendTo( "#images" );
        if ( i === 3 ) {
          return false;
        }
		*/