dashboard.service('Auth', function() {
    var user = window.user;
	if(typeof(user) == 'undefined' && typeof($.cookie('user')) != 'undefined'){
		user = JSON.parse($.cookie('user'));
	}
	
    return {
        getUser: function() {
            return user;
        },
        setUser: function(newUser) {
            user = newUser;
			$.cookie('user', JSON.stringify(user));
        },
        isConnected: function() {
            return !!user;
        }
    };
});