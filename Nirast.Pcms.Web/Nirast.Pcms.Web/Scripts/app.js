$(function () {
    'use strict';

	
    /*****************************************************
     * Scroll Top
     *****************************************************/
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('#scroll-top').fadeIn();
        } else {
            $('#scroll-top').fadeOut();
        }
    });
    $("a[href='#top']").on('click', function() {
        $("html, body").animate({ scrollTop: 0 }, "slow");
        return false;
    });	
	
    /*****************************************************
     * Advance Search
     *****************************************************/
	
	$(".ad-search").click(function(){
		$(this).next('.dropdown-menu').slideToggle();
	});
	$(".ad-close").click(function(){
		$(this).parents('.dropdown-menu').slideToggle();
	});
	
	
    /*****************************************************
     * Fixed header Editer's Note
     *****************************************************/
	$(window).scroll(function(){
	  var sticky = $('body'),
		  scroll = $(window).scrollTop();
	
	  if (scroll >= 0) sticky.addClass('stick');
	  else sticky.removeClass('stick');
    });
    $(window).scroll(function(){
        var sticky1 = $('.editors-note'),
            scroll = $(window).scrollTop();
      
        if (scroll >= 100) sticky1.addClass('stick');
        else sticky1.removeClass('stick');
      });
      
	
	
    /*****************************************************
     * Search Tile View
     *****************************************************/
    // $('#btnList').click(function(){
	// 	$('#btnTile').removeClass('active');
	// 	$('body').removeClass('search-tile');
	// 	$(this).addClass('active');
		
	// 	$('.masonry').masonry({
	// 	  itemSelector: '.search-col-wrap',
	// 	});
	//  });
	//  $('#btnTile').click(function(){
	// 	$('#btnList').removeClass('active');
	// 	$('body').addClass('search-tile');
	// 	$(this).addClass('active');
		
	// 	$('.masonry').masonry({
	// 	  itemSelector: '.search-col-wrap',
	// 	});
	//  });
    /*****************************************************
     * Tooltip
     *****************************************************/
	 
	 $('[data-toggle="tooltip"]').tooltip();
	 
    /*****************************************************
     * Single File Uploader
     *****************************************************/
	 
	 $('#FileUploaderSingle').removeAttr('multiple');
	 
    /*****************************************************
     * Fixed Table Header
     *****************************************************/	 
	 
    // $('.dashboard-tbl table').fixedHeaderTable();
	 
    /*****************************************************
     * CKEDITOR
     *****************************************************/
	
	//CKEDITOR.replace('ckeditor');
// 	CKEDITOR.editorConfig = function( config ) {
// 	config.language = 'es';
// 	config.height = 250;
// 	config.toolbarCanCollapse = true;
// 	config.toolbar= [
// 		{ name: 'basicstyles', items: [ 'Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'Superscript', '-', 'CopyFormatting', 'RemoveFormat' ] },
// 		{ name: 'paragraph', items: [ 'NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', '-', 'Blockquote', 'CreateDiv', '-', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock', '-', 'BidiLtr', 'BidiRtl', 'Language' ] },
// 					 ]
// };
// CKEDITOR.instances.editor1.on('change', function() { 
//     console.log("TEST");
//    $('#hdnDescription').val(CKEDITOR.instances.editor1.getData());
// });
	
    /*****************************************************
     * File Control
     *****************************************************/
	$('.file-control').change(function(){
		var VAL = $(this).val();
		$('.file-text').val(VAL);
    });
    
    

});