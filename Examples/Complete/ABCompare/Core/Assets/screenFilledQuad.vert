#version 300 es

in vec2 fuUV;
in vec3 fuVertex;
out vec2 vUV;



//Failures!
//vUV = fuVertex.xy + 0.5;
//vUV = vec2(fuVertex.x + fuVertex.y + 0.5, fuVertex.y + 0.4 );
//vUV = vec2(fuVertex.x  , fuVertex.y+ 0.5); 

void main() 
{
	
    vUV = fuUV; 
	
    gl_Position = vec4(fuVertex * 2.0,1.0); //=> Vertex Position in clip space
}