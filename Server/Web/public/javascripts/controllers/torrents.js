dashboard.controller('torrents', function ($scope, torrentsModel) {

    $scope.loading = false;
    $scope.search = function(text){
        $scope.loading = true;
        torrentsModel.search(text, function(data){
            $scope.loading = false;
            if (data.status == 'ok') {
                $scope.results = data.data.movies;
            }
        });
    }
    
    $scope.download = function(url){
        $scope.loading = true;
        torrentsModel.download(url, function(result){
            $scope.loading = false;
            $scope.$apply();
            $scope.results = [];
        });
    }
    
});