var scene;
var camera;
var renderer;
var controls;
var viewModel;
var itemMaterial;

async function PackContainers(request) {
	return $.ajax({
		url: '/api/containerpacking',
		type: 'POST',
		data: request,
		contentType: 'application/json; charset=utf-8'
	});
};

function InitializeDrawing() {
	var container = $('#drawing-container');

	scene = new THREE.Scene();
	camera = new THREE.PerspectiveCamera( 50, window.innerWidth/window.innerHeight, 0.1, 1000 );
	camera.lookAt(scene.position);

	//var axisHelper = new THREE.AxisHelper( 5 );
	//scene.add( axisHelper );

	// LIGHT
	var light = new THREE.PointLight(0xffffff);
	light.position.set(0,150,100);
	scene.add(light);

	// Get the item stuff ready.
	itemMaterial = new THREE.MeshNormalMaterial( { transparent: true, opacity: 0.6 } );

	renderer = new THREE.WebGLRenderer( { antialias: true } ); // WebGLRenderer CanvasRenderer
	renderer.setClearColor( 0xf0f0f0 );
	renderer.setPixelRatio( window.devicePixelRatio );
	renderer.setSize( window.innerWidth / 2, window.innerHeight / 2);
	container.append( renderer.domElement );

	controls = new THREE.OrbitControls( camera, renderer.domElement );
	window.addEventListener( 'resize', onWindowResize, false );

	animate();
};

function onWindowResize() {
	camera.aspect = window.innerWidth / window.innerHeight;
	camera.updateProjectionMatrix();
	renderer.setSize( window.innerWidth / 2, window.innerHeight / 2 );
}
//
function animate() {
	requestAnimationFrame( animate );
	controls.update();
	render();
}
function render() {
	renderer.render( scene, camera );
}

var ViewModel = function () {
	var self = this;

	self.ItemCounter = 0;
	self.ContainerCounter = 0;

	self.ItemsToRender = ko.observableArray([]);
	self.LastItemRenderedIndex = ko.observable(-1);

	self.ContainerOriginOffset = {
		x: 0,
		y: 0,
		z: 0
	};

	self.AlgorithmsToUse = ko.observableArray([]);
	self.ItemsToPack = ko.observableArray([]);
	self.Containers = ko.observableArray([]);

	self.NewItemToPack = ko.mapping.fromJS(new ItemToPack());
	self.NewContainer = ko.mapping.fromJS(new Container());

	self.GenerateItemsToPack = function () {
		self.ItemsToPack([]);
		//----------------------------------------------------------------------------------------------------------------------------------------
		self.ItemsToPack.push(ko.mapping.fromJS({ ID: 11, Name: '11', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 7, IsStackable: true }));
		self.ItemsToPack.push(ko.mapping.fromJS({ ID: 8, Name: '8', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 6, IsStackable: true }));
		self.ItemsToPack.push(ko.mapping.fromJS({ ID: 9, Name: '9', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 7, IsStackable: true }));
		self.ItemsToPack.push(ko.mapping.fromJS({ ID: 10, Name: '10', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 5, IsStackable: true }));
		self.ItemsToPack.push(ko.mapping.fromJS({ ID: 13, Name: '13', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 2, IsStackable: true }));
		self.ItemsToPack.push(ko.mapping.fromJS({ ID: 12, Name: '12', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 2, IsStackable: true }));
		self.ItemsToPack.push(ko.mapping.fromJS({ ID: 14, Name: '14', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 1, IsStackable: true }));
		self.ItemsToPack.push(ko.mapping.fromJS({ ID: 3, Name: '3', Length: 13.5, Width: 10.4, Height: 12.7, Quantity: 1, IsStackable: true }));
		self.ItemsToPack.push(ko.mapping.fromJS({ ID: 1, Name: '1', Length: 13.5, Width: 10.4, Height: 12.7, Quantity: 1, IsStackable: true }));
		self.ItemsToPack.push(ko.mapping.fromJS({ ID: 16, Name: '16', Length: 13.5, Width: 10.4, Height: 14.0, Quantity: 1, IsStackable: true }));
		self.ItemsToPack.push(ko.mapping.fromJS({ ID: 2, Name: '2', Length: 13.5, Width: 10.4, Height: 12.7, Quantity: 1, IsStackable: true }));
		self.ItemsToPack.push(ko.mapping.fromJS({ ID: 4, Name: '4', Length: 12.2, Width: 11.4, Height: 8.6, Quantity: 8, IsStackable: true }));
		self.ItemsToPack.push(ko.mapping.fromJS({ ID: 5, Name: '5', Length: 12.2, Width: 11.4, Height: 8.6, Quantity: 7, IsStackable: true }));
		self.ItemsToPack.push(ko.mapping.fromJS({ ID: 6, Name: '6', Length: 12.2, Width: 11.4, Height: 8.6, Quantity: 6, IsStackable: true }));
		self.ItemsToPack.push(ko.mapping.fromJS({ ID: 7, Name: '7', Length: 12.2, Width: 11.4, Height: 8.6, Quantity: 8, IsStackable: true }));

		//----------------------------------------------------------------------------------------------------------------------------------------
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 32, Name: '32', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 1, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 33, Name: '33', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 1, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 34, Name: '34', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 1, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 29, Name: '29', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 6, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 30, Name: '30', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 3, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 26, Name: '26', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 6, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 27, Name: '27', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 6, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 28, Name: '28', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 7, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 31, Name: '31', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 1, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 22, Name: '22', Length: 12.2, Width: 11.4, Height: 8.6, Quantity: 6, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 18, Name: '18', Length: 13.5, Width: 10.4, Height: 12.7, Quantity: 1, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 19, Name: '19', Length: 13.5, Width: 10.4, Height: 12.7, Quantity: 1, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 20, Name: '20', Length: 13.5, Width: 10.4, Height: 12.7, Quantity: 1, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 21, Name: '21', Length: 13.5, Width: 10.4, Height: 12.7, Quantity: 1, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 23, Name: '23', Length: 12.2, Width: 11.4, Height: 8.6, Quantity: 6, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 24, Name: '24', Length: 12.2, Width: 11.4, Height: 8.6, Quantity: 6, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 25, Name: '25', Length: 12.2, Width: 11.4, Height: 8.6, Quantity: 6, IsStackable: true }));

		//----------------------------------------------------------------------------------------------------------------------------------------
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 47, Name: '47', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 6, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 46, Name: '46', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 7, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 43, Name: '43', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 7, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 44, Name: '44', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 7, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 45, Name: '45', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 6, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 48, Name: '48', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 2, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 49, Name: '49', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 2, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 50, Name: '50', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 2, IsStackable: true }));

		//----------------------------------------------------------------------------------------------------------------------------------------
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 68, Name: '68', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 5, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 67, Name: '67', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 6, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 69, Name: '69', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 1, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 71, Name: '71', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 2, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 70, Name: '70', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 1, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 72, Name: '72', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 1, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 64, Name: '64', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 6, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 65, Name: '65', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 6, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 66, Name: '66', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 7, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 63, Name: '63', Length: 12.2, Width: 11.4, Height: 8.6, Quantity: 6, IsStackable: true }));

		//----------------------------------------------------------------------------------------------------------------------------------------
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 90, Name: '90', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 7, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 77, Name: '77', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 3, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 89, Name: '89', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 6, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 86, Name: '86', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 6, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 87, Name: '87', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 6, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 88, Name: '88', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 6, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 91, Name: '91', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 3, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 92, Name: '92', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 3, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 93, Name: '93', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 2, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 82, Name: '82', Length: 12.2, Width: 11.4, Height: 8.6, Quantity: 6, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 94, Name: '94', Length: 17.8, Width: 12.2, Height: 8.6, Quantity: 3, IsStackable: true }));
		// self.ItemsToPack.push(ko.mapping.fromJS({ ID: 83, Name: '83', Length: 12.2, Width: 11.4, Height: 8.6, Quantity: 6, IsStackable: true }));
	};
	self.GenerateContainers = function () {
		self.Containers([]);
		self.Containers.push(ko.mapping.fromJS({ ID: 1000, Name: 'Trailer', Length: 161.5, Width: 25.9, Height: 27.4, AlgorithmPackingResults: [] }));
	};

	self.AddAlgorithmToUse = function () {
		var algorithmID = $('#algorithm-select option:selected').val();
		var algorithmName = $('#algorithm-select option:selected').text();
		self.AlgorithmsToUse.push({ AlgorithmID: algorithmID, AlgorithmName: algorithmName });
	};

	self.RemoveAlgorithmToUse = function (item) {
		self.AlgorithmsToUse.remove(item);
	};

	self.AddNewItemToPack = function () {
		self.NewItemToPack.ID(self.ItemCounter++);
		self.ItemsToPack.push(ko.mapping.fromJS(ko.mapping.toJS(self.NewItemToPack)));
		self.NewItemToPack.Name('');
		self.NewItemToPack.Length('');
		self.NewItemToPack.Width('');
		self.NewItemToPack.Height('');
		self.NewItemToPack.Quantity('');
		self.NewItemToPack.IsStackable(true);
	};

	self.RemoveItemToPack = function (item) {
		self.ItemsToPack.remove(item);
	};

	self.AddNewContainer = function () {
		self.NewContainer.ID(self.ContainerCounter++);
		self.Containers.push(ko.mapping.fromJS(ko.mapping.toJS(self.NewContainer)));
		self.NewContainer.Name('');
		self.NewContainer.Length('');
		self.NewContainer.Width('');
		self.NewContainer.Height('');
	};

	self.RemoveContainer = function (item) {
		self.Containers.remove(item);
	};

	self.PackContainers = function () {
		var algorithmsToUse = [];

		self.AlgorithmsToUse().forEach(algorithm => {
			algorithmsToUse.push(algorithm.AlgorithmID);
		});
		
		var itemsToPack = [];

		self.ItemsToPack().forEach(item => {
			var itemToPack = {
				ID: item.ID(),
				Dim1: item.Length(),
				Dim2: item.Width(),
				Dim3: item.Height(),
				Quantity: item.Quantity(),
				IsStackable: item.IsStackable()
			};
			
			itemsToPack.push(itemToPack);
		});
		
		var containers = [];

		// Send a packing request for each container in the list.
		self.Containers().forEach(container => {
			var containerToUse = {
				ID: container.ID(),
				Length: container.Length(),
				Width: container.Width(),
				Height: container.Height()
			};

			containers.push(containerToUse);
		});
		
		// Build container packing request.
		var request = {
			Containers: containers,
			ItemsToPack: itemsToPack,
			AlgorithmTypeIDs: algorithmsToUse
		};
		
		PackContainers(JSON.stringify(request))
			.then(response => {
				// Tie this response back to the correct containers.
				response.forEach(containerPackingResult => {
					self.Containers().forEach(container => {
						if (container.ID() == containerPackingResult.ContainerID) {
							container.AlgorithmPackingResults(containerPackingResult.AlgorithmPackingResults);
						}
					});
				});
			});
	};
	
	self.ShowPackingView = function (algorithmPackingResult) {
		var container = this;
		var selectedObject = scene.getObjectByName('container');
		scene.remove( selectedObject );
		
		for (var i = 0; i < 1000; i++) {
			var selectedObject = scene.getObjectByName('cube' + i);
			scene.remove(selectedObject);
		}
		
		camera.position.set(container.Length(), container.Length(), container.Length());

		self.ItemsToRender(algorithmPackingResult.PackedItems);
		self.LastItemRenderedIndex(-1);

		self.ContainerOriginOffset.x = -1 * container.Length() / 2;
		self.ContainerOriginOffset.y = -1 * container.Height() / 2;
		self.ContainerOriginOffset.z = -1 * container.Width() / 2;

		var geometry = new THREE.BoxGeometry(container.Length(), container.Height(), container.Width());
		var geo = new THREE.EdgesGeometry( geometry ); // or WireframeGeometry( geometry )
		var mat = new THREE.LineBasicMaterial( { color: 0x000000, linewidth: 2 } );
		var wireframe = new THREE.LineSegments( geo, mat );
		wireframe.position.set(0, 0, 0);
		wireframe.name = 'container';
		scene.add( wireframe );
	};

	self.AreItemsPacked = function () {
		if (self.LastItemRenderedIndex() > -1) {
			return true;
		}

		return false;
	};

	self.AreAllItemsPacked = function () {
		if (self.ItemsToRender().length === self.LastItemRenderedIndex() + 1) {
			return true;
		}

		return false;
	};

	self.PackItemInRender = function () {
		var itemIndex = self.LastItemRenderedIndex() + 1;

		var itemOriginOffset = {
			x: self.ItemsToRender()[itemIndex].PackDimX / 2,
			y: self.ItemsToRender()[itemIndex].PackDimY / 2,
			z: self.ItemsToRender()[itemIndex].PackDimZ / 2
		};

		var itemGeometry = new THREE.BoxGeometry(self.ItemsToRender()[itemIndex].PackDimX, self.ItemsToRender()[itemIndex].PackDimY, self.ItemsToRender()[itemIndex].PackDimZ);
		var cube = new THREE.Mesh(itemGeometry, itemMaterial);
		cube.position.set(self.ContainerOriginOffset.x + itemOriginOffset.x + self.ItemsToRender()[itemIndex].CoordX, self.ContainerOriginOffset.y + itemOriginOffset.y + self.ItemsToRender()[itemIndex].CoordY, self.ContainerOriginOffset.z + itemOriginOffset.z + self.ItemsToRender()[itemIndex].CoordZ);
		cube.name = 'cube' + itemIndex;
		scene.add( cube );

		self.LastItemRenderedIndex(itemIndex);
	};

	self.UnpackItemInRender = function () {
		var selectedObject = scene.getObjectByName('cube' + self.LastItemRenderedIndex());
		scene.remove( selectedObject );
		self.LastItemRenderedIndex(self.LastItemRenderedIndex() - 1);
	};
};

var ItemToPack = function () {
	this.ID = '';
	this.Name = '';
	this.Length = '';
	this.Width = '';
	this.Height = '',
	this.Quantity = '';
	this.IsStackable = true;
}

var Container = function () {
	this.ID = '';
	this.Name = '';
	this.Length = '';
	this.Width = '';
	this.Height = '';
	this.AlgorithmPackingResults = [];
}

$(document).ready(() => {
	$('[data-toggle="tooltip"]').tooltip(); 
	InitializeDrawing();

	viewModel = new ViewModel();
	ko.applyBindings(viewModel);
});