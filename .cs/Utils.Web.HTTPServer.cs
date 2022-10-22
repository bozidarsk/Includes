#if UNITY
using UnityEngine;
#endif

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Utils.Web 
{
	public class HTTPServer 
	{
		public string Version { private set; get; }
		public string Name { private set; get; }
		public string CurrentDirectory { private set; get; }

		public string HostName { private set; get; }
		public int Port { private set; get; }
		public bool IsRunning { private set; get; }

		public delegate Response ResponseMethod(HTTPServer server, Request request);
		private ResponseMethod responseMethod;

		private string lastHTML;
		private TcpListener listener;
		private Thread thread;

		public static readonly Dictionary<string, string> MimeTypes = new Dictionary<string, string>() 
		{
			{ ".html", "text/html" },
			{ ".css", "text/css" },
			{ ".js", "text/javascript" },
			{ ".ico", "image/x-icon" },
			{ ".png", "image/png" },
			{ ".jpg", "image/jpeg" },
			{ ".jpeg", "image/jpeg" },
			{ ".gif", "image/gif" },
			{ ".txt", "text/plain" },
			{ ".json", "application/json" },
			{ ".csv", "text/csv" },				
			{ ".mp4", "video/mp4" },
			{ ".mkv", "video/x-matroska" },
			{ ".mp3", "audio/mpeg" },
			{ ".wav", "audio/wav" },
			{ ".ttf", "font/ttf" },
			{ ".htm", "text/html" },
			{ ".ez", "application/andrew-inset" },
			{ ".aw", "application/applixware" },
			{ ".atom", "application/atom+xml" },
			{ ".atomcat", "application/atomcat+xml" },
			{ ".atomsvc", "application/atomsvc+xml" },
			{ ".ccxml", "application/ccxml+xml" },
			{ ".cu", "application/cu-seeme" },
			{ ".davmount", "application/davmount+xml" },
			{ ".ecma", "application/ecmascript" },
			{ ".emma", "application/emma+xml" },
			{ ".epub", "application/epub+zip" },
			{ ".pfr", "application/font-tdpfr" },
			{ ".gz", "application/gzip" },
			{ ".stk", "application/hyperstudio" },
			{ ".jar", "application/java-archive" },
			{ ".ser", "application/java-serialized-object" },
			{ ".class", "application/java-vm" },
			{ ".lostxml", "application/lost+xml" },
			{ ".hqx", "application/mac-binhex40" },
			{ ".cpt", "application/mac-compactpro" },
			{ ".mrc", "application/marc" },
			{ ".ma", "application/mathematica" },
			{ ".mbox", "application/mbox" },
			{ ".mscml", "application/mediaservercontrol+xml" },
			{ ".mp4s", "application/mp4" },
			{ ".doc", "application/msword" },
			{ ".mxf", "application/mxf" },
			{ ".a", "application/octet-stream" },
			{ ".oda", "application/oda" },
			{ ".opf", "application/oebps-package+xml" },
			{ ".ogx", "application/ogg" },
			{ ".onepkg", "application/onenote" },
			{ ".xer", "application/patch-ops-error+xml" },
			{ ".pdf", "application/pdf" },
			{ ".pgp", "application/pgp-encrypted" },
			{ ".asc", "application/pgp-signature" },
			{ ".prf", "application/pics-rules" },
			{ ".p10", "application/pkcs10" },
			{ ".p7c", "application/pkcs7-mime" },
			{ ".p7s", "application/pkcs7-signature" },
			{ ".cer", "application/pkix-cert" },
			{ ".crl", "application/pkix-crl" },
			{ ".pkipath", "application/pkix-pkipath" },
			{ ".pki", "application/pkixcmp" },
			{ ".pls", "application/pls+xml" },
			{ ".ai", "application/postscript" },
			{ ".cww", "application/prs.cww" },
			{ ".rdf", "application/rdf+xml" },
			{ ".rif", "application/reginfo+xml" },
			{ ".rnc", "application/relax-ng-compact-syntax" },
			{ ".rl", "application/resource-lists+xml" },
			{ ".rld", "application/resource-lists-diff+xml" },
			{ ".rs", "application/rls-services+xml" },
			{ ".rsd", "application/rsd+xml" },
			{ ".rss", "application/rss+xml" },
			{ ".rtf", "application/rtf" },
			{ ".sbml", "application/sbml+xml" },
			{ ".scq", "application/scvp-cv-request" },
			{ ".scs", "application/scvp-cv-response" },
			{ ".spq", "application/scvp-vp-request" },
			{ ".spp", "application/scvp-vp-response" },
			{ ".sdp", "application/sdp" },
			{ ".setpay", "application/set-payment-initiation" },
			{ ".setreg", "application/set-registration-initiation" },
			{ ".shf", "application/shf+xml" },
			{ ".smi", "application/smil+xml" },
			{ ".rq", "application/sparql-query" },
			{ ".srx", "application/sparql-results+xml" },
			{ ".gram", "application/srgs" },
			{ ".grxml", "application/srgs+xml" },
			{ ".ssml", "application/ssml+xml" },
			{ ".plb", "application/vnd.3gpp.pic-bw-large" },
			{ ".psb", "application/vnd.3gpp.pic-bw-small" },
			{ ".pvb", "application/vnd.3gpp.pic-bw-var" },
			{ ".tcap", "application/vnd.3gpp2.tcap" },
			{ ".pwn", "application/vnd.3m.post-it-notes" },
			{ ".aso", "application/vnd.accpac.simply.aso" },
			{ ".imp", "application/vnd.accpac.simply.imp" },
			{ ".acu", "application/vnd.acucobol" },
			{ ".acutc", "application/vnd.acucorp" },
			{ ".air", "application/vnd.adobe.air-application-installer-package+zip" },
			{ ".xdp", "application/vnd.adobe.xdp+xml" },
			{ ".xfdf", "application/vnd.adobe.xfdf" },
			{ ".azf", "application/vnd.airzip.filesecure.azf" },
			{ ".azs", "application/vnd.airzip.filesecure.azs" },
			{ ".azw", "application/vnd.amazon.ebook" },
			{ ".acc", "application/vnd.americandynamics.acc" },
			{ ".ami", "application/vnd.amiga.ami" },
			{ ".apk", "application/vnd.android.package-archive" },
			{ ".cii", "application/vnd.anser-web-certificate-issue-initiation" },
			{ ".fti", "application/vnd.anser-web-funds-transfer-initiation" },
			{ ".atx", "application/vnd.antix.game-component" },
			{ ".mpkg", "application/vnd.apple.installer+xml" },
			{ ".swi", "application/vnd.arastra.swi" },
			{ ".aep", "application/vnd.audiograph" },
			{ ".mpm", "application/vnd.blueice.multipass" },
			{ ".bmi", "application/vnd.bmi" },
			{ ".rep", "application/vnd.businessobjects" },
			{ ".cdxml", "application/vnd.chemdraw+xml" },
			{ ".mmd", "application/vnd.chipnuts.karaoke-mmd" },
			{ ".cdy", "application/vnd.cinderella" },
			{ ".cla", "application/vnd.claymore" },
			{ ".c4d", "application/vnd.clonk.c4group" },
			{ ".csp", "application/vnd.commonspace" },
			{ ".cdbcmsg", "application/vnd.contact.cmsg" },
			{ ".cmc", "application/vnd.cosmocaller" },
			{ ".clkx", "application/vnd.crick.clicker" },
			{ ".clkk", "application/vnd.crick.clicker.keyboard" },
			{ ".clkp", "application/vnd.crick.clicker.palette" },
			{ ".clkt", "application/vnd.crick.clicker.template" },
			{ ".clkw", "application/vnd.crick.clicker.wordbank" },
			{ ".wbs", "application/vnd.criticaltools.wbs+xml" },
			{ ".pml", "application/vnd.ctc-posml" },
			{ ".ppd", "application/vnd.cups-ppd" },
			{ ".car", "application/vnd.curl.car" },
			{ ".pcurl", "application/vnd.curl.pcurl" },
			{ ".rdz", "application/vnd.data-vision.rdz" },
			{ ".fe_launch", "application/vnd.denovo.fcselayout-link" },
			{ ".dna", "application/vnd.dna" },
			{ ".mlp", "application/vnd.dolby.mlp" },
			{ ".dpg", "application/vnd.dpgraph" },
			{ ".dfac", "application/vnd.dreamfactory" },
			{ ".geo", "application/vnd.dynageo" },
			{ ".mag", "application/vnd.ecowin.chart" },
			{ ".nml", "application/vnd.enliven" },
			{ ".esf", "application/vnd.epson.esf" },
			{ ".msf", "application/vnd.epson.msf" },
			{ ".qam", "application/vnd.epson.quickanime" },
			{ ".slt", "application/vnd.epson.salt" },
			{ ".ssf", "application/vnd.epson.ssf" },
			{ ".es3", "application/vnd.eszigno3+xml" },
			{ ".ez2", "application/vnd.ezpix-album" },
			{ ".ez3", "application/vnd.ezpix-package" },
			{ ".fdf", "application/vnd.fdf" },
			{ ".mseed", "application/vnd.fdsn.mseed" },
			{ ".dataless", "application/vnd.fdsn.seed" },
			{ ".gph", "application/vnd.flographit" },
			{ ".ftc", "application/vnd.fluxtime.clip" },
			{ ".bin", "application/octet-stream" },
			{ ".book", "application/vnd.framemaker" },
			{ ".fnc", "application/vnd.frogans.fnc" },
			{ ".ltf", "application/vnd.frogans.ltf" },
			{ ".fsc", "application/vnd.fsc.weblaunch" },
			{ ".oas", "application/vnd.fujitsu.oasys" },
			{ ".oa2", "application/vnd.fujitsu.oasys2" },
			{ ".oa3", "application/vnd.fujitsu.oasys3" },
			{ ".fg5", "application/vnd.fujitsu.oasysgp" },
			{ ".bh2", "application/vnd.fujitsu.oasysprs" },
			{ ".ddd", "application/vnd.fujixerox.ddd" },
			{ ".xdw", "application/vnd.fujixerox.docuworks" },
			{ ".xbd", "application/vnd.fujixerox.docuworks.binder" },
			{ ".fzs", "application/vnd.fuzzysheet" },
			{ ".txd", "application/vnd.genomatix.tuxedo" },
			{ ".ggb", "application/vnd.geogebra.file" },
			{ ".ggt", "application/vnd.geogebra.tool" },
			{ ".gex", "application/vnd.geometry-explorer" },
			{ ".gmx", "application/vnd.gmx" },
			{ ".kml", "application/vnd.google-earth.kml+xml" },
			{ ".kmz", "application/vnd.google-earth.kmz" },
			{ ".gqf", "application/vnd.grafeq" },
			{ ".gac", "application/vnd.groove-account" },
			{ ".ghf", "application/vnd.groove-help" },
			{ ".gim", "application/vnd.groove-identity-message" },
			{ ".grv", "application/vnd.groove-injector" },
			{ ".gtm", "application/vnd.groove-tool-message" },
			{ ".tpl", "application/vnd.groove-tool-template" },
			{ ".vcg", "application/vnd.groove-vcard" },
			{ ".zmm", "application/vnd.handheld-entertainment+xml" },
			{ ".hbci", "application/vnd.hbci" },
			{ ".les", "application/vnd.hhe.lesson-player" },
			{ ".hpgl", "application/vnd.hp-hpgl" },
			{ ".hpid", "application/vnd.hp-hpid" },
			{ ".hps", "application/vnd.hp-hps" },
			{ ".jlt", "application/vnd.hp-jlyt" },
			{ ".pcl", "application/vnd.hp-pcl" },
			{ ".pclxl", "application/vnd.hp-pclxl" },
			{ ".php", "application/x-httpd-php" },
			{ ".ppt", "application/vnd.ms-powerpoint" },
			{ ".sfd-hdstx", "application/vnd.hydrostatix.sof-data" },
			{ ".x3d", "application/vnd.hzn-3d-crossword" },
			{ ".mpy", "application/vnd.ibm.minipay" },
			{ ".afp", "application/vnd.ibm.modcap" },
			{ ".irm", "application/vnd.ibm.rights-management" },
			{ ".sc", "application/vnd.ibm.secure-container" },
			{ ".icc", "application/vnd.iccprofile" },
			{ ".igl", "application/vnd.igloader" },
			{ ".ivp", "application/vnd.immervision-ivp" },
			{ ".ivu", "application/vnd.immervision-ivu" },
			{ ".xpw", "application/vnd.intercon.formnet" },
			{ ".qbo", "application/vnd.intu.qbo" },
			{ ".qfx", "application/vnd.intu.qfx" },
			{ ".rar", "application/vnd.rar" },
			{ ".rcprofile", "application/vnd.ipunplugged.rcprofile" },
			{ ".irp", "application/vnd.irepository.package+xml" },
			{ ".xpr", "application/vnd.is-xpr" },
			{ ".jam", "application/vnd.jam" },
			{ ".rms", "application/vnd.jcp.javame.midlet-rms" },
			{ ".jisp", "application/vnd.jisp" },
			{ ".joda", "application/vnd.joost.joda-archive" },
			{ ".ktr", "application/vnd.kahootz" },
			{ ".karbon", "application/vnd.kde.karbon" },
			{ ".chrt", "application/vnd.kde.kchart" },
			{ ".kfo", "application/vnd.kde.kformula" },
			{ ".flw", "application/vnd.kde.kivio" },
			{ ".kon", "application/vnd.kde.kontour" },
			{ ".kpr", "application/vnd.kde.kpresenter" },
			{ ".ksp", "application/vnd.kde.kspread" },
			{ ".kwd", "application/vnd.kde.kword" },
			{ ".htke", "application/vnd.kenameaapp" },
			{ ".kia", "application/vnd.kidspiration" },
			{ ".kne", "application/vnd.kinar" },
			{ ".skd", "application/vnd.koan" },
			{ ".sse", "application/vnd.kodak-descriptor" },
			{ ".lbd", "application/vnd.llamagraphics.life-balance.desktop" },
			{ ".lbe", "application/vnd.llamagraphics.life-balance.exchange+xml" },
			{ ".123", "application/vnd.lotus-1-2-3" },
			{ ".apr", "application/vnd.lotus-approach" },
			{ ".pre", "application/vnd.lotus-freelance" },
			{ ".nsf", "application/vnd.lotus-notes" },
			{ ".org", "application/vnd.lotus-organizer" },
			{ ".scm", "application/vnd.lotus-screencam" },
			{ ".lwp", "application/vnd.lotus-wordpro" },
			{ ".portpkg", "application/vnd.macports.portpkg" },
			{ ".mcd", "application/vnd.mcd" },
			{ ".mc1", "application/vnd.medcalcdata" },
			{ ".cdkey", "application/vnd.mediastation.cdkey" },
			{ ".mwf", "application/vnd.mfer" },
			{ ".mfm", "application/vnd.mfmp" },
			{ ".flo", "application/vnd.micrografx.flo" },
			{ ".igx", "application/vnd.micrografx.igx" },
			{ ".mif", "application/vnd.mif" },
			{ ".daf", "application/vnd.mobius.daf" },
			{ ".dis", "application/vnd.mobius.dis" },
			{ ".mbk", "application/vnd.mobius.mbk" },
			{ ".mqy", "application/vnd.mobius.mqy" },
			{ ".msl", "application/vnd.mobius.msl" },
			{ ".plc", "application/vnd.mobius.plc" },
			{ ".txf", "application/vnd.mobius.txf" },
			{ ".mpn", "application/vnd.mophun.application" },
			{ ".mpc", "application/vnd.mophun.certificate" },
			{ ".xul", "application/vnd.mozilla.xul+xml" },
			{ ".cil", "application/vnd.ms-artgalry" },
			{ ".cab", "application/vnd.ms-cab-compressed" },
			{ ".xla", "application/vnd.ms-excel" },
			{ ".xls", "application/vnd.ms-excel" },
			{ ".xlam", "application/vnd.ms-excel.addin.macroenabled.12" },
			{ ".xlsb", "application/vnd.ms-excel.sheet.binary.macroenabled.12" },
			{ ".xlsm", "application/vnd.ms-excel.sheet.macroenabled.12" },
			{ ".xltm", "application/vnd.ms-excel.template.macroenabled.12" },
			{ ".eot", "application/vnd.ms-fontobject" },
			{ ".chm", "application/vnd.ms-htmlhelp" },
			{ ".ims", "application/vnd.ms-ims" },
			{ ".lrm", "application/vnd.ms-lrm" },
			{ ".cat", "application/vnd.ms-pki.seccat" },
			{ ".stl", "application/vnd.ms-pki.stl" },
			{ ".pot", "application/vnd.ms-powerpoint" },
			{ ".ppam", "application/vnd.ms-powerpoint.addin.macroenabled.12" },
			{ ".pptm", "application/vnd.ms-powerpoint.presentation.macroenabled.12" },
			{ ".sldm", "application/vnd.ms-powerpoint.slide.macroenabled.12" },
			{ ".ppsm", "application/vnd.ms-powerpoint.slideshow.macroenabled.12" },
			{ ".potm", "application/vnd.ms-powerpoint.template.macroenabled.12" },
			{ ".mpp", "application/vnd.ms-project" },
			{ ".docm", "application/vnd.ms-word.document.macroenabled.12" },
			{ ".dotm", "application/vnd.ms-word.template.macroenabled.12" },
			{ ".wcm", "application/vnd.ms-works" },
			{ ".wpl", "application/vnd.ms-wpl" },
			{ ".xps", "application/vnd.ms-xpsdocument" },
			{ ".mseq", "application/vnd.mseq" },
			{ ".mus", "application/vnd.musician" },
			{ ".msty", "application/vnd.muvee.style" },
			{ ".nlu", "application/vnd.neurolanguage.nlu" },
			{ ".nnd", "application/vnd.noblenet-directory" },
			{ ".nns", "application/vnd.noblenet-sealer" },
			{ ".nnw", "application/vnd.noblenet-web" },
			{ ".ngdat", "application/vnd.nokia.n-gage.data" },
			{ ".n-gage", "application/vnd.nokia.n-gage.symbian.install" },
			{ ".rpst", "application/vnd.nokia.radio-preset" },
			{ ".rpss", "application/vnd.nokia.radio-presets" },
			{ ".edm", "application/vnd.novadigm.edm" },
			{ ".edx", "application/vnd.novadigm.edx" },
			{ ".ext", "application/vnd.novadigm.ext" },
			{ ".odc", "application/vnd.oasis.opendocument.chart" },
			{ ".otc", "application/vnd.oasis.opendocument.chart-template" },
			{ ".odb", "application/vnd.oasis.opendocument.database" },
			{ ".odf", "application/vnd.oasis.opendocument.formula" },
			{ ".odft", "application/vnd.oasis.opendocument.formula-template" },
			{ ".odg", "application/vnd.oasis.opendocument.graphics" },
			{ ".otg", "application/vnd.oasis.opendocument.graphics-template" },
			{ ".odi", "application/vnd.oasis.opendocument.image" },
			{ ".oti", "application/vnd.oasis.opendocument.image-template" },
			{ ".odp", "application/vnd.oasis.opendocument.presentation" },
			{ ".otp", "application/vnd.oasis.opendocument.presentation-template" },
			{ ".ods", "application/vnd.oasis.opendocument.spreadsheet" },
			{ ".ots", "application/vnd.oasis.opendocument.spreadsheet-template" },
			{ ".odt", "application/vnd.oasis.opendocument.text" },
			{ ".otm", "application/vnd.oasis.opendocument.text-master" },
			{ ".ott", "application/vnd.oasis.opendocument.text-template" },
			{ ".oth", "application/vnd.oasis.opendocument.text-web" },
			{ ".xo", "application/vnd.olpc-sugar" },
			{ ".dd2", "application/vnd.oma.dd2+xml" },
			{ ".oxt", "application/vnd.openofficeorg.extension" },
			{ ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
			{ ".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide" },
			{ ".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow" },
			{ ".potx", "application/vnd.openxmlformats-officedocument.presentationml.template" },
			{ ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
			{ ".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template" },
			{ ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
			{ ".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template" },
			{ ".dp", "application/vnd.osgi.dp" },
			{ ".oprc", "application/vnd.palm" },
			{ ".str", "application/vnd.pg.format" },
			{ ".ei6", "application/vnd.pg.osasli" },
			{ ".efif", "application/vnd.picsel" },
			{ ".plf", "application/vnd.pocketlearn" },
			{ ".pbd", "application/vnd.powerbuilder6" },
			{ ".box", "application/vnd.previewsystems.box" },
			{ ".mgz", "application/vnd.proteus.magazine" },
			{ ".qps", "application/vnd.publishare-delta-tree" },
			{ ".ptid", "application/vnd.pvi.ptid1" },
			{ ".qwd", "application/vnd.quark.quarkxpress" },
			{ ".mxl", "application/vnd.recordare.musicxml" },
			{ ".musicxml", "application/vnd.recordare.musicxml+xml" },
			{ ".cod", "application/vnd.rim.cod" },
			{ ".rm", "application/vnd.rn-realmedia" },
			{ ".link66", "application/vnd.route66.link66+xml" },
			{ ".see", "application/vnd.seemail" },
			{ ".sema", "application/vnd.sema" },
			{ ".semd", "application/vnd.semd" },
			{ ".semf", "application/vnd.semf" },
			{ ".ifm", "application/vnd.shana.informed.formdata" },
			{ ".itp", "application/vnd.shana.informed.formtemplate" },
			{ ".iif", "application/vnd.shana.informed.interchange" },
			{ ".ipk", "application/vnd.shana.informed.package" },
			{ ".twd", "application/vnd.simtech-mindmapper" },
			{ ".mmf", "application/vnd.smaf" },
			{ ".teacher", "application/vnd.smart.teacher" },
			{ ".sdkd", "application/vnd.solent.sdkm+xml" },
			{ ".dxp", "application/vnd.spotfire.dxp" },
			{ ".sfs", "application/vnd.spotfire.sfs" },
			{ ".db", "application/vnd.sqlite3" },
			{ ".sdc", "application/vnd.stardivision.calc" },
			{ ".sda", "application/vnd.stardivision.draw" },
			{ ".sdd", "application/vnd.stardivision.impress" },
			{ ".smf", "application/vnd.stardivision.math" },
			{ ".sdw", "application/vnd.stardivision.writer" },
			{ ".sgl", "application/vnd.stardivision.writer-global" },
			{ ".sxc", "application/vnd.sun.xml.calc" },
			{ ".stc", "application/vnd.sun.xml.calc.template" },
			{ ".sxd", "application/vnd.sun.xml.draw" },
			{ ".std", "application/vnd.sun.xml.draw.template" },
			{ ".sxi", "application/vnd.sun.xml.impress" },
			{ ".sti", "application/vnd.sun.xml.impress.template" },
			{ ".sxm", "application/vnd.sun.xml.math" },
			{ ".sxw", "application/vnd.sun.xml.writer" },
			{ ".sxg", "application/vnd.sun.xml.writer.global" },
			{ ".stw", "application/vnd.sun.xml.writer.template" },
			{ ".sus", "application/vnd.sus-calendar" },
			{ ".svd", "application/vnd.svd" },
			{ ".sis", "application/vnd.symbian.install" },
			{ ".xsm", "application/vnd.syncml+xml" },
			{ ".bdm", "application/vnd.syncml.dm+wbxml" },
			{ ".xdm", "application/vnd.syncml.dm+xml" },
			{ ".tao", "application/vnd.tao.intent-module-archive" },
			{ ".tmo", "application/vnd.tmobile-livetv" },
			{ ".tpt", "application/vnd.trid.tpt" },
			{ ".mxs", "application/vnd.triscape.mxs" },
			{ ".tra", "application/vnd.trueapp" },
			{ ".ufd", "application/vnd.ufdl" },
			{ ".utz", "application/vnd.uiq.theme" },
			{ ".umj", "application/vnd.umajin" },
			{ ".unityweb", "application/vnd.unity" },
			{ ".uoml", "application/vnd.uoml+xml" },
			{ ".vcx", "application/vnd.vcx" },
			{ ".vsd", "application/vnd.visio" },
			{ ".vis", "application/vnd.visionary" },
			{ ".vsf", "application/vnd.vsf" },
			{ ".sic", "application/vnd.wap.sic" },
			{ ".slc", "application/vnd.wap.slc" },
			{ ".wbxml", "application/vnd.wap.wbxml" },
			{ ".wmlc", "application/vnd.wap.wmlc" },
			{ ".wmlsc", "application/vnd.wap.wmlscriptc" },
			{ ".wtb", "application/vnd.webturbo" },
			{ ".wpd", "application/vnd.wordperfect" },
			{ ".wqd", "application/vnd.wqd" },
			{ ".stf", "application/vnd.wt.stf" },
			{ ".xar", "application/vnd.xara" },
			{ ".xfdl", "application/vnd.xfdl" },
			{ ".hvd", "application/vnd.yamaha.hv-dic" },
			{ ".hvs", "application/vnd.yamaha.hv-script" },
			{ ".hvp", "application/vnd.yamaha.hv-voice" },
			{ ".osf", "application/vnd.yamaha.openscoreformat" },
			{ ".osfpvg", "application/vnd.yamaha.openscoreformat.osfpvg+xml" },
			{ ".saf", "application/vnd.yamaha.smaf-audio" },
			{ ".spf", "application/vnd.yamaha.smaf-phrase" },
			{ ".cmp", "application/vnd.yellowriver-custom-menu" },
			{ ".zir", "application/vnd.zul" },
			{ ".zaz", "application/vnd.zzazz.deck+xml" },
			{ ".vxml", "application/voicexml+xml" },
			{ ".hlp", "application/winhlp" },
			{ ".wsdl", "application/wsdl+xml" },
			{ ".wspolicy", "application/wspolicy+xml" },
			{ ".7z", "application/x-7z-compressed" },
			{ ".abw", "application/x-abiword" },
			{ ".ace", "application/x-ace-compressed" },
			{ ".aab", "application/x-authorware-bin" },
			{ ".aam", "application/x-authorware-map" },
			{ ".aas", "application/x-authorware-seg" },
			{ ".bcpio", "application/x-bcpio" },
			{ ".torrent", "application/x-bittorrent" },
			{ ".bz", "application/x-bzip" },
			{ ".boz", "application/x-bzip2" },
			{ ".vcd", "application/x-cdlink" },
			{ ".chat", "application/x-chat" },
			{ ".pgn", "application/x-chess-pgn" },
			{ ".cpio", "application/x-cpio" },
			{ ".csh", "application/x-csh" },
			{ ".deb", "application/x-debian-package" },
			{ ".cct", "application/x-director" },
			{ ".wad", "application/x-doom" },
			{ ".ncx", "application/x-dtbncx+xml" },
			{ ".dtb", "application/x-dtbook+xml" },
			{ ".res", "application/x-dtbresource+xml" },
			{ ".dvi", "application/x-dvi" },
			{ ".bdf", "application/x-font-bdf" },
			{ ".gsf", "application/x-font-ghostscript" },
			{ ".psf", "application/x-font-linux-psf" },
			{ ".pcf", "application/x-font-pcf" },
			{ ".snf", "application/x-font-snf" },
			{ ".ttc", "application/x-font-ttf" },
			{ ".afm", "application/x-font-type1" },
			{ ".spl", "application/x-futuresplash" },
			{ ".gnumeric", "application/x-gnumeric" },
			{ ".gtar", "application/x-gtar" },
			{ ".hdf", "application/x-hdf" },
			{ ".jnlp", "application/x-java-jnlp-file" },
			{ ".kil", "application/x-killustrator" },
			{ ".latex", "application/x-latex" },
			{ ".mobi", "application/x-mobipocket-ebook" },
			{ ".application", "application/x-ms-application" },
			{ ".wmd", "application/x-ms-wmd" },
			{ ".wmz", "application/x-ms-wmz" },
			{ ".xbap", "application/x-ms-xbap" },
			{ ".mdb", "application/x-msaccess" },
			{ ".obd", "application/x-msbinder" },
			{ ".crd", "application/x-mscardfile" },
			{ ".clp", "application/x-msclip" },
			{ ".bat", "application/x-msdownload" },
			{ ".m13", "application/x-msmediaview" },
			{ ".wmf", "application/x-msmetafile" },
			{ ".mny", "application/x-msmoney" },
			{ ".pub", "application/x-mspublisher" },
			{ ".scd", "application/x-msschedule" },
			{ ".trm", "application/x-msterminal" },
			{ ".wri", "application/x-mswrite" },
			{ ".cdf", "application/x-netcdf" },
			{ ".pm", "application/x-perl" },
			{ ".p12", "application/x-pkcs12" },
			{ ".p7b", "application/x-pkcs7-certificates" },
			{ ".pyc", "application/x-python-code" },
			{ ".rpa", "application/x-redhat-package-manager" },
			{ ".rpm", "application/x-rpm" },
			{ ".sh", "application/x-sh" },
			{ ".shar", "application/x-shar" },
			{ ".swf", "application/x-shockwave-flash" },
			{ ".xap", "application/x-silverlight-app" },
			{ ".sit", "application/x-stuffit" },
			{ ".sitx", "application/x-stuffitx" },
			{ ".sv4cpio", "application/x-sv4cpio" },
			{ ".sv4crc", "application/x-sv4crc" },
			{ ".tar", "application/x-tar" },
			{ ".tcl", "application/x-tcl" },
			{ ".tex", "application/x-tex" },
			{ ".tfm", "application/x-tex-tfm" },
			{ ".texi", "application/x-texinfo" },
			{ ".ustar", "application/x-ustar" },
			{ ".src", "application/x-wais-source" },
			{ ".crt", "application/x-x509-ca-cert" },
			{ ".fig", "application/x-xfig" },
			{ ".xpi", "application/x-xpinstall" },
			{ ".xenc", "application/xenc+xml" },
			{ ".xht", "application/xhtml+xml" },
			{ ".xml", "application/xml" },
			{ ".dtd", "application/xml-dtd" },
			{ ".xop", "application/xop+xml" },
			{ ".xslt", "application/xslt+xml" },
			{ ".xspf", "application/xspf+xml" },
			{ ".mxml", "application/xv+xml" },
			{ ".zip", "application/zip" },
			{ ".3gp", "audio/3gpp" },
			{ ".3g2", "audio/3gpp2" },
			{ ".adp", "audio/adpcm" },
			{ ".aiff", "audio/aiff" },
			{ ".au", "audio/basic" },
			{ ".kar", "audio/midi" },
			{ ".mp4a", "audio/mp4" },
			{ ".m2a", "audio/mpeg" },
			{ ".oga", "audio/ogg" },
			{ ".opus", "audio/opus" },
			{ ".eol", "audio/vnd.digital-winds" },
			{ ".dts", "audio/vnd.dts" },
			{ ".dtshd", "audio/vnd.dts.hd" },
			{ ".lvp", "audio/vnd.lucent.voice" },
			{ ".pya", "audio/vnd.ms-playready.media.pya" },
			{ ".ecelp4800", "audio/vnd.nuera.ecelp4800" },
			{ ".ecelp7470", "audio/vnd.nuera.ecelp7470" },
			{ ".ecelp9600", "audio/vnd.nuera.ecelp9600" },
			{ ".weba", "audio/webm" },
			{ ".aac", "audio/x-aac" },
			{ ".aif", "audio/x-aiff" },
			{ ".mka", "audio/x-matroska" },
			{ ".m3u", "audio/x-mpegurl" },
			{ ".wax", "audio/x-ms-wax" },
			{ ".wma", "audio/x-ms-wma" },
			{ ".ra", "audio/x-pn-realaudio" },
			{ ".rmp", "audio/x-pn-realaudio-plugin" },
			{ ".cdx", "chemical/x-cdx" },
			{ ".cif", "chemical/x-cif" },
			{ ".cmdf", "chemical/x-cmdf" },
			{ ".cml", "chemical/x-cml" },
			{ ".csml", "chemical/x-csml" },
			{ ".xyz", "chemical/x-xyz" },
			{ ".otf", "font/otf" },
			{ ".woff", "font/woff" },
			{ ".woff2", "font/woff2" },
			{ ".gcode", "gcode" },
			{ ".avif", "image/avif" },
			{ ".bmp", "image/bmp" },
			{ ".cgm", "image/cgm" },
			{ ".g3", "image/g3fax" },
			{ ".heif", "image/heic" },
			{ ".ief", "image/ief" },
			{ ".jpe", "image/jpeg" },
			{ ".btif", "image/prs.btif" },
			{ ".svg", "image/svg+xml" },
			{ ".tif", "image/tiff" },
			{ ".psd", "image/vnd.adobe.photoshop" },
			{ ".djv", "image/vnd.djvu" },
			{ ".dwg", "image/vnd.dwg" },
			{ ".dxf", "image/vnd.dxf" },
			{ ".fbs", "image/vnd.fastbidsheet" },
			{ ".fpx", "image/vnd.fpx" },
			{ ".fst", "image/vnd.fst" },
			{ ".mmr", "image/vnd.fujixerox.edmics-mmr" },
			{ ".rlc", "image/vnd.fujixerox.edmics-rlc" },
			{ ".mdi", "image/vnd.ms-modi" },
			{ ".npx", "image/vnd.net-fpx" },
			{ ".wbmp", "image/vnd.wap.wbmp" },
			{ ".xif", "image/vnd.xiff" },
			{ ".webp", "image/webp" },
			{ ".dng", "image/x-adobe-dng" },
			{ ".cr2", "image/x-canon-cr2" },
			{ ".crw", "image/x-canon-crw" },
			{ ".ras", "image/x-cmu-raster" },
			{ ".cmx", "image/x-cmx" },
			{ ".erf", "image/x-epson-erf" },
			{ ".fh", "image/x-freehand" },
			{ ".raf", "image/x-fuji-raf" },
			{ ".dcr", "image/x-kodak-dcr" },
			{ ".k25", "image/x-kodak-k25" },
			{ ".kdc", "image/x-kodak-kdc" },
			{ ".mrw", "image/x-minolta-mrw" },
			{ ".nef", "image/x-nikon-nef" },
			{ ".orf", "image/x-olympus-orf" },
			{ ".raw", "image/x-panasonic-raw" },
			{ ".pcx", "image/x-pcx" },
			{ ".pef", "image/x-pentax-pef" },
			{ ".pct", "image/x-pict" },
			{ ".pnm", "image/x-portable-anymap" },
			{ ".pbm", "image/x-portable-bitmap" },
			{ ".pgm", "image/x-portable-graymap" },
			{ ".ppm", "image/x-portable-pixmap" },
			{ ".rgb", "image/x-rgb" },
			{ ".x3f", "image/x-sigma-x3f" },
			{ ".arw", "image/x-sony-arw" },
			{ ".sr2", "image/x-sony-sr2" },
			{ ".srf", "image/x-sony-srf" },
			{ ".xbm", "image/x-xbitmap" },
			{ ".xpm", "image/x-xpixmap" },
			{ ".xwd", "image/x-xwindowdump" },
			{ ".eml", "message/rfc822" },
			{ ".iges", "model/iges" },
			{ ".mesh", "model/mesh" },
			{ ".dwf", "model/vnd.dwf" },
			{ ".gdl", "model/vnd.gdl" },
			{ ".gtw", "model/vnd.gtw" },
			{ ".mts", "model/vnd.mts" },
			{ ".vtu", "model/vnd.vtu" },
			{ ".vrml", "model/vrml" },
			{ ".ics", "text/calendar" },
			{ ".md", "text/markdown" },
			{ ".mathml", "text/mathml" },
			{ ".conf", "text/plain" },
			{ ".dsc", "text/prs.lines.tag" },
			{ ".rtx", "text/richtext" },
			{ ".sgm", "text/sgml" },
			{ ".tsv", "text/tab-separated-values" },
			{ ".man", "text/troff" },
			{ ".uri", "text/uri-list" },
			{ ".curl", "text/vnd.curl" },
			{ ".dcurl", "text/vnd.curl.dcurl" },
			{ ".mcurl", "text/vnd.curl.mcurl" },
			{ ".scurl", "text/vnd.curl.scurl" },
			{ ".fly", "text/vnd.fly" },
			{ ".flx", "text/vnd.fmi.flexstor" },
			{ ".gv", "text/vnd.graphviz" },
			{ ".3dml", "text/vnd.in3d.3dml" },
			{ ".spot", "text/vnd.in3d.spot" },
			{ ".jad", "text/vnd.sun.j2me.app-descriptor" },
			{ ".si", "text/vnd.wap.si" },
			{ ".sl", "text/vnd.wap.sl" },
			{ ".wml", "text/vnd.wap.wml" },
			{ ".wmls", "text/vnd.wap.wmlscript" },
			{ ".asm", "text/x-asm" },
			{ ".c", "text/x-c" },
			{ ".f", "text/x-fortran" },
			{ ".java", "text/x-java-source" },
			{ ".p", "text/x-pascal" },
			{ ".py", "text/x-python" },
			{ ".etx", "text/x-setext" },
			{ ".uu", "text/x-uuencode" },
			{ ".vcs", "text/x-vcalendar" },
			{ ".vcf", "text/x-vcard" },
			{ ".h261", "video/h261" },
			{ ".h263", "video/h263" },
			{ ".h264", "video/h264" },
			{ ".jpgv", "video/jpeg" },
			{ ".jpgm", "video/jpm" },
			{ ".mj2", "video/mj2" },
			{ ".m1v", "video/mpeg" },
			{ ".ogv", "video/ogg" },
			{ ".mov", "video/quicktime" },
			{ ".fvt", "video/vnd.fvt" },
			{ ".m4u", "video/vnd.mpegurl" },
			{ ".pyv", "video/vnd.ms-playready.media.pyv" },
			{ ".viv", "video/vnd.vivo" },
			{ ".webm", "video/webm" },
			{ ".f4v", "video/x-f4v" },
			{ ".fli", "video/x-fli" },
			{ ".flv", "video/x-flv" },
			{ ".m4v", "video/x-m4v" },
			{ ".asf", "video/x-ms-asf" },
			{ ".wm", "video/x-ms-wm" },
			{ ".wmv", "video/x-ms-wmv" },
			{ ".wmx", "video/x-ms-wmx" },
			{ ".wvx", "video/x-ms-wvx" },
			{ ".avi", "video/x-msvideo" },
			{ ".movie", "video/x-sgi-movie" },
			{ ".ice", "x-conference/x-cooltalk" }
		};

		private void HandleClient(TcpClient client) 
		{
			if (client == null) { return; }

			StreamReader reader = new StreamReader(client.GetStream());
			string message = "";

			while (reader.Peek() != -1) { message += reader.ReadLine() + "\n"; }

			Request request = Request.GenerateRequest(message);
			Response response = responseMethod(this, request);

			int errorCode = Convert.ToInt32(response.Status.Split(' ')[0]);
			string color = (errorCode >= 400) ? ColorConsole.RedF : ((errorCode == 200) ? ColorConsole.GreenF : ColorConsole.GrayF);

			if (request != null) { ColorConsole.WriteLine("Request: " + request.Host + request.URL + " - Response: " + response.Mime + " " + color + errorCode.ToString() + ColorConsole.GrayF); }
			else { ColorConsole.WriteLine(ColorConsole.RedF + "Null request." + ColorConsole.GrayF); }
			response.Post(this, client.GetStream());
		}

		private void Run() 
		{
			IsRunning = true;
			listener.Start();

			while (IsRunning) 
			{
				if (listener == null) { IsRunning = false; return; }
				TcpClient client = listener.AcceptTcpClient();
				HandleClient(client);
				if (client != null) { client.Close(); }
			}

			listener.Stop();
			IsRunning = false;
		}

		public void Start() { thread.Start(); Console.WriteLine("Server started on " + HostName + ":" + Port.ToString()); }
		public void Stop() { thread.Abort(); Console.WriteLine("Server stopped."); IsRunning = false; }

		public HTTPServer(
			ResponseMethod responseMethod = null,
			string CurrentDirectory = null,
			int Port = 8000,
			string HostName = null,
			string Name = null
		) {
			this.Version = "HTTP/1.1";
			this.Name = (Name != null) ? Name : "HTTP Server";
			this.HostName = (HostName != null) ? HostName : "127.0.0.1";
			this.Port = Port;
			this.CurrentDirectory = (CurrentDirectory != null) ? CurrentDirectory : Directory.GetCurrentDirectory();
			this.responseMethod = (responseMethod != null) ? responseMethod : new ResponseMethod(Response.GenerateDirectoryResponse);
			this.listener = new TcpListener(WebTools.ConvertIP(this.HostName), this.Port);
			this.thread = new Thread(new ThreadStart(Run));
		}



		public class Request 
		{
			public string Type { private set; get; }
			public string URL { private set; get; }
			public string Host { private set; get; }

			public static Request GenerateRequest(string request) 
			{
				if (request == "" || request == null) { return null; }

				string[] tokens = request.Split(' ', '\n');
				string type = tokens[0];
				string url = tokens[1];
				string host = tokens[4];

				return new Request(type, url, host);
			}

			public Request() {}
			public Request(string Type, string URL, string Host) 
			{
				this.Type = Type;
				this.URL = URL;
				this.Host = Host;
			}
		}



		public class Response 
		{
			public string Status { set; get; }
			public string Mime { set; get; }
			public byte[] Data { set; get; }
			public string Headers { private set; get; }

			public static Response OK(string message = "") { return new Response("200 OK", "text/html", Encoding.ASCII.GetBytes("<h1>200 OK</h1><p>" + message + "</p>")); }
			public static Response BadRequest(string message = "") { return new Response("400 Bad Request", "text/html", Encoding.ASCII.GetBytes("<h1>400 Bad Request</h1><p>" + message + "</p>")); }
			public static Response Forbidden(string message = "") { return new Response("403 Forbidden", "text/html", Encoding.ASCII.GetBytes("<h1>403 Forbidden</h1><p>" + message + "</p>")); }
			public static Response NotFound(string message = "") { return new Response("404 Not Found", "text/html", Encoding.ASCII.GetBytes("<h1>404 Not Found</h1><p>" + message + "</p>")); }
			public static Response MethodNotAllowed(string message = "") { return new Response("405 Method Not Allowed", "text/html", Encoding.ASCII.GetBytes("<h1>405 Method Not Allowed</h1><p>" + message + "</p>")); }
			public static Response NotAcceptable(string message = "") { return new Response("406 Not Acceptable", "text/html", Encoding.ASCII.GetBytes("<h1>406 Not Acceptable</h1><p>" + message + "</p>")); }
			public static Response UnsupportedMediaType(string message = "") { return new Response("415 Unsupported Media Type", "text/html", Encoding.ASCII.GetBytes("<h1>415 Unsupported Media Type</h1><p>" + message + "</p>")); }
			public static Response NotImplemented(string message = "") { return new Response("501 Not Implemented", "text/html", Encoding.ASCII.GetBytes("<h1>501 Not Implemented</h1><p>" + message + "</p>")); }

			public static Response GenerateHTMLResponse(HTTPServer server, Request request) 
			{
				if (request == null) { return Response.BadRequest("request == null"); }

				if (request.Type == "GET") 
				{
					Response response = Response.OK();
					int paramsIndex = request.URL.IndexOf("?");
					string path = (paramsIndex >= 0) ? request.URL.Remove(paramsIndex) : request.URL;
					if (path[path.Length - 1] == '/') { path = path.TrimEnd('/'); }
					path = WebTools.ReplaceURLChars(path.Replace("/", "\\"));

					int dotIndex = path.LastIndexOf(".");
					string dir = server.CurrentDirectory + path;
					if (File.Exists(dir) || dotIndex >= 0) 
					{
						string extention = (dotIndex >= 0) ? path.Substring(dotIndex, path.Length - dotIndex).ToLower() : "";
						try { response.Mime = HTTPServer.MimeTypes[extention]; }
						catch (KeyNotFoundException) 
						{
							ColorConsole.WriteLine(ColorConsole.YellowF + "MIME not avaliable for URL: " + request.URL + ColorConsole.GrayF);
							return Response.NotAcceptable("MIME not found for '" + request.URL + "'.");
						}

						string file = dir;
						if (!File.Exists(file)) 
						{
							file = file.Replace(path, server.lastHTML + path);
							if (!File.Exists(file)) { return Response.NotFound("File not found. - '" + file + "'"); }
						}

						response.Data = File.ReadAllBytes(file);
						return response;
					}
					else 
					{
						if (!Directory.Exists(dir)) { return Response.NotFound("Directory not found. - '" + dir + "'"); }
						string[] files = { "index.html", "index.htm", "default.html", "default.htm" };

						for (int i = 0; i < files.Length; i++) { if (File.Exists(dir + files[i])) { response.Data = File.ReadAllBytes(dir + files[i]); server.lastHTML = path; return response; } }

						files = Directory.GetFiles(dir, "*.html", SearchOption.TopDirectoryOnly);
						if (files.Length > 0) { response.Data = File.ReadAllBytes(files[0]); server.lastHTML = path; return response; }

						files = Directory.GetFiles(dir, "*.htm", SearchOption.TopDirectoryOnly);
						if (files.Length > 0) { response.Data = File.ReadAllBytes(files[0]); server.lastHTML = path; return response; }

						return Response.NotFound("HTML file not found.");
					}
				}
				
				return Response.MethodNotAllowed("Only GET method is allowed.");
			}

			public static Response GenerateDirectoryResponse(HTTPServer server, Request request) 
			{
				if (request == null) { return Response.BadRequest(); }

				if (request.Type == "GET") 
				{
					Response response = Response.OK();
					int paramsIndex = request.URL.IndexOf("?");
					string path = (paramsIndex >= 0) ? request.URL.Remove(paramsIndex) : request.URL;
					if (path[path.Length - 1] == '/') { path = path.TrimEnd('/'); }
					path = WebTools.ReplaceURLChars(path.Replace("/", "\\"));

					if (File.Exists(server.CurrentDirectory + path)) 
					{
						int dotIndex = path.LastIndexOf(".");
						string extention = (dotIndex >= 0) ? path.Substring(dotIndex, path.Length - dotIndex).ToLower() : "";
						try { response.Mime = HTTPServer.MimeTypes[extention]; }
						catch (KeyNotFoundException) 
						{
							ColorConsole.WriteLine(ColorConsole.YellowF + "MIME not avaliable for URL: " + request.URL + ColorConsole.GrayF);
							response.Mime = "text/html";
						}

						string file = server.CurrentDirectory + path;
						if (!File.Exists(file)) 
						{
							file = file.Replace(server.CurrentDirectory, server.lastHTML);
							if (!File.Exists(file)) { return Response.NotFound("File not found. - '" + file + "'"); }
						}

						if (response.Mime == "application/pdf") { response.AddHeader("Content-Disposition: inline"); response.Data = File.ReadAllBytes(file); return response; }
						if (response.Mime.StartsWith("image/")) { response.Data = File.ReadAllBytes(file); return response; }
						if (response.Mime.StartsWith("audio/")) { response.Mime = "audio/mpeg"; response.Data = File.ReadAllBytes(file); return response; }
						if (response.Mime.StartsWith("video/")) { response.Mime = "video/mp4"; response.Data = File.ReadAllBytes(file); return response; }
						if (extention == ".exe" || extention == ".dll" || extention == ".bin" ||
							extention == ".doc" || extention == ".ppt" || extention == ".xls" ||
							extention == ".docx" || extention == ".pptx" || extention == ".xlsx" ||
							extention == ".blend" || extention == ".deb" || extention == ".psd" ||
							extention == ".rar" || extention == ".zip" || extention == ".7z" ||
							extention == ".lnk" || extention == ".out")
						{ response.Mime = "text/plain"; response.Data = File.ReadAllBytes(file); return response; }

						response.Mime = "text/html";
						response.Data = Encoding.UTF8.GetBytes
						(
							"<html><head>" +
							"<meta content=\"text/html; charset=utf-8\" http-equiv=\"Content-Type\">" +
							"<meta name=\"color-scheme\" content=\"light dark\">" +
							"</head><body>" +
							"<p style=\"font-family: monospace; word-wrap: break-word; white-space: pre-wrap;\">"+
							File.ReadAllText(file).Replace("<", "&lt;").Replace(">", "&gt;") +
							"</p></body></html>"
						);

						return response;
					}
					else 
					{
						string dir = server.CurrentDirectory + path;
						if (!Directory.Exists(dir)) { return Response.NotFound(((dir.LastIndexOf(".") - dir.LastIndexOf("\\") > 1) ? "File" : "Directory") + " not found. - '" + dir + "'"); }

						bool isRoot = server.CurrentDirectory == dir;
						int t = (!isRoot) ? 1 : 0;
						string[] files = Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly);
						string[] directories = Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly);
						string[] items = new string[files.Length + directories.Length + t];

						if (!isRoot) { items[0] = " "; }
						for (int i = 0; i < files.Length; i++) { items[t++] = files[i].Replace(dir + "\\", ""); }
						for (int i = 0; i < directories.Length; i++) { items[t++] = directories[i].Replace(dir + "\\", ""); }
						Array.Sort(items, (x, y) => String.Compare(x, y));
						if (!isRoot) { items[0] = ".."; }

						string output = 
						"<html><head>" +
						"<meta content=\"text/html; charset=utf-8\" http-equiv=\"Content-Type\">" +
						"<meta name=\"color-scheme\" content=\"light dark\">" +
						"</head><body>" +
						"<p style=\"font-family: monospace; word-wrap: break-word; white-space: pre-wrap;\">";
						for (int i = 0; i < items.Length; i++) 
						{
							bool isURL = items[i].EndsWith(".url");
							output += "<a href=\"" +
							((!isURL) ? (path + "\\" + items[i]) : File.ReadAllText(dir + "\\" + items[i]).Replace("[InternetShortcut]\x000d\x000aURL=", "")) +
							"\"" + ((isURL) ? " target=\"_blank\"" : "") + ">" + items[i] +
							((Directory.Exists(dir + "\\" + items[i])) ? "/" : "") +
							"</a>\n";
						}

						output += "</p></body></html>";
						response.Data = Encoding.UTF8.GetBytes(output);
						return response;
					}
				}
				
				return Response.MethodNotAllowed("Only GET method is allowed.");
			}

			public void AddHeader(string header) { Headers += header + "\r\n"; }
			public void Post(HTTPServer server, NetworkStream stream) 
			{
				StreamWriter writer = new StreamWriter(stream);
				Headers = server.Version + " " + Status + "\r\n" + Headers;
				AddHeader("Content-Type: " + Mime + "; charset=utf-8");
				AddHeader("Accept-Ranges: bytes");
				AddHeader("Content-Length: " + Data.Length.ToString());
				writer.WriteLine(Headers);
				writer.Flush();

				try { stream.Write(Data, 0, Data.Length); }
				catch (System.IO.IOException) {}
			}

			public Response() {}
			public Response(string Status, string Mime, byte[] Data) 
			{
				this.Status = Status;
				this.Mime = Mime;
				this.Data = Data;
				this.Headers = "";
			}
		}
	}
}