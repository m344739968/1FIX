  $(function() {
            $('#slides').slides({
                preload: true,
                preloadImage: '',
                play: 3000,
                pause: 2500,
                hoverPause: true,
                generatePagination: false,
                //pagination: false,
                animationStart: function(current) {
                    $('.caption').animate({
							bottom: -1
                    }, 100);
                    if (window.console && console.log) {
                        // example return of current slide number
                        console.log('animationStart on slide: ', current);
                    };
                },
                animationComplete: function(current) {
                    $('.caption').animate({
                        bottom: 0
                    }, 200);
                    if (window.console && console.log) {
                        // example return of current slide number
                        console.log('animationComplete on slide: ', current);
                    };
                },
                slidesLoaded: function() {
                    $('.caption').animate({
                        bottom: 0
                    }, 200);
                }
            });
        });    