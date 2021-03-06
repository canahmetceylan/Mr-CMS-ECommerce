/*
 * Project: Twitter Bootstrap Hover Dropdown
 * Author: Cameron Spear
 * Contributors: Mattia Larentis
 *
 * Dependencies?: Twitter Bootstrap's Dropdown plugin
 *
 * A simple plugin to enable twitter bootstrap dropdowns to active on hover and provide a nice user experience.
 *
 * No license, do what you want. I'd love credit or a shoutout, though.
 *
 * http://cameronspear.com/blog/twitter-bootstrap-dropdown-on-hover-plugin/
 */(function (e, t, n) { var r = e(); e.fn.dropdownHover = function (n) { r = r.add(this.parent()); return this.each(function () { var s = e(this), o = s.parent(), u = { delay: 500, instantlyCloseOthers: !0 }, a = { delay: e(this).data("delay"), instantlyCloseOthers: e(this).data("close-others") }, f = e.extend(!0, {}, u, n, a), l; o.hover(function (e) { if (!o.hasClass("open") && !s.is(e.target)) return !0; if (i()) { f.instantlyCloseOthers === !0 && r.removeClass("open"); t.clearTimeout(l); o.addClass("open") } }, function () { i() && (l = t.setTimeout(function () { o.removeClass("open") }, f.delay)) }); s.hover(function () { if (i()) { f.instantlyCloseOthers === !0 && r.removeClass("open"); t.clearTimeout(l); o.addClass("open") } }); o.find(".dropdown-submenu").each(function () { var n = e(this), r; n.hover(function () { if (i()) { t.clearTimeout(r); n.children(".dropdown-menu").show(); n.siblings().children(".dropdown-menu").hide() } }, function () { var e = n.children(".dropdown-menu"); i() ? r = t.setTimeout(function () { e.hide() }, f.delay) : e.hide() }) }) }) }; var i = function () { return !e("#cwspear-is-awesome").is(":visible") }; e(document).ready(function () { e('[data-hover="dropdown"]').dropdownHover(); e('<div class="navbar" style="visibility:hidden;position:fixed"><div class="btn-navbar" id="cwspear-is-awesome">.</div></div>').appendTo("body") }); var s = ".dropdown-submenu:hover>.dropdown-menu{display:none}", o = document.createElement("style"); o.type = "text/css"; o.styleSheet ? o.styleSheet.cssText = s : o.appendChild(document.createTextNode(s)); e("head")[0].appendChild(o) })(jQuery, this);