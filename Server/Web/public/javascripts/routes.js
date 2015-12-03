dashboard.config(function($routeProvider, $locationProvider) {
  $routeProvider
   .when('/', {
    templateUrl: 'main.html',
    resolve: {
        connected: checkIsConnected
    }
  })
  .when('/login', {
    templateUrl: 'login.html'	
  });

  // configure html5 to get links working on jsfiddle
  $locationProvider.html5Mode(true);
});

var checkIsConnected = function($q, $timeout, $http, $location) {
    var deferred = $q.defer();
 
	var user = $.cookie('user');
	if (typeof(user) != 'undefined') {
		$timeout(deferred.resolve, 0);
	} else {
		$timeout(deferred.reject, 0);
		$location.url('/login');
	}
 
    return deferred.promise;
};